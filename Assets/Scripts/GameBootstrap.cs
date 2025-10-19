using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    public static SaveSystem.SaveData loadedData;

    void Start()
    {
        // --- ロードデータがある場合のみ適用 ---
        if (loadedData != null)
        {
            Debug.Log("[GameBootstrap] ロードデータ適用を開始");

            // プレイヤーの位置と向きを復元
            var player = FindFirstObjectByType<GridMovement>();
            if (player != null)
            {
                player.transform.position = loadedData.playerPosition;
                player.SetDirection(loadedData.playerDirection);
            }
            else
            {
                Debug.LogWarning("[GameBootstrap] プレイヤーが見つかりませんでした。");
            }

            // --- インベントリ復元 ---
            if (InventoryManager.Instance != null && loadedData.inventoryData != null)
            {
                InventoryManager.Instance.LoadData(
                    new InventorySaveData
                    {
                        ownedItemIDs = new System.Collections.Generic.List<string>(
                            loadedData.inventoryData.ownedItemIDs
                        )
                    }
                );
            }

            // --- 資料復元 ---
            if (DocumentManager.Instance != null && loadedData.documentData != null)
            {
                DocumentManager.Instance.LoadData(
                    new DocumentSaveData
                    {
                        obtainedIDs = new System.Collections.Generic.List<string>(
                            loadedData.documentData.obtainedIDs
                        )
                    }
                );
            }

            // --- フラグ復元 ---
            if (GameFlags.Instance != null && loadedData.flagData != null)
            {
                GameFlags.Instance.LoadFlags(
                    new FlagSaveData
                    {
                        activeFlags = loadedData.flagData.activeFlags
                    }
                );
            }

            // --- Titleシーンのアンロードを遅延で実行 ---
            StartCoroutine(UnloadTitleAfterLoad());

            // --- カメラ確認（Bootstrap由来を維持） ---
            Camera cam = Camera.main;
            if (cam == null)
            {
                var newCam = new GameObject("MainCamera");
                cam = newCam.AddComponent<Camera>();
                cam.tag = "MainCamera";
                cam.transform.position = new Vector3(
                    loadedData.playerPosition.x,
                    loadedData.playerPosition.y,
                    -10f
                );
                newCam.AddComponent<AudioListener>();
                Debug.Log("[GameBootstrap] カメラがなかったため新規生成しました。");
            }
            else
            {
                if (cam.GetComponent<AudioListener>() == null)
                {
                    cam.gameObject.AddComponent<AudioListener>();
                    Debug.Log("[GameBootstrap] 既存カメラにAudioListenerを追加しました。");
                }
            }

            Debug.Log("[GameBootstrap] ロードデータ適用完了。");
            loadedData = null; // 一度使ったら破棄
        }
    }

    // ==============================
    // Titleシーンを確実にアンロード
    // ==============================
    private IEnumerator UnloadTitleAfterLoad()
    {
        // 少し待たないとUnityがシーン情報を更新しない
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            string sceneName = s.name;

            // 部分一致でも検出
            if (sceneName.Contains("Title"))
            {
                Debug.Log($"[GameBootstrap] Titleシーン({sceneName})をアンロードします。");
                yield return SceneManager.UnloadSceneAsync(s);
            }
        }

        // Additiveロードしたゲームシーンをアクティブ化
        Scene newScene = SceneManager.GetSceneByName("Scenes0");
        if (newScene.IsValid())
        {
            SceneManager.SetActiveScene(newScene);
            Debug.Log($"[GameBootstrap] アクティブシーンを {newScene.name} に変更しました。");
        }
        else
        {
            Debug.LogWarning("[GameBootstrap] ゲームシーン(Scenes0)が見つかりませんでした。");
        }
    }
}