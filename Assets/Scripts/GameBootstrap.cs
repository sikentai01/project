using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// セーブデータをロードしてゲーム状態を完全復元する。
/// シーン名・座標・向き・フラグ・ギミック・アイテムを正確に適用。
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    public static SaveSystem.SaveData loadedData;

    private IEnumerator Start()
    {
        if (loadedData == null)
        {
            Debug.LogWarning("[GameBootstrap] loadedData がありません。適用スキップ。");
            Destroy(gameObject);
            yield break;
        }

        // === シーン名をもとに正しいシーンをロード ===
        string savedSceneName = loadedData.sceneName;
        if (!string.IsNullOrEmpty(savedSceneName))
        {
            Debug.Log($"[GameBootstrap] セーブされたシーン '{savedSceneName}' を読み込み中...");

            var savedScene = SceneManager.GetSceneByName(savedSceneName);
            if (!savedScene.isLoaded)
            {
                var async = SceneManager.LoadSceneAsync(savedSceneName, LoadSceneMode.Additive);
                while (!async.isDone) yield return null;
                savedScene = SceneManager.GetSceneByName(savedSceneName);
                Debug.Log($"[GameBootstrap] {savedSceneName} を Additiveロード完了");
            }

            // シーンをアクティブ化
            SceneManager.SetActiveScene(savedScene);
            yield return new WaitForEndOfFrame();
        }

        // ===== インベントリ・ドキュメント =====
        if (InventoryManager.Instance != null && loadedData.inventoryData != null)
            InventoryManager.Instance.LoadData(loadedData.inventoryData);

        if (DocumentManager.Instance != null && loadedData.documentData != null)
            DocumentManager.Instance.LoadData(loadedData.documentData);

        // ===== フラグ =====
        if (GameFlags.Instance != null)
        {
            GameFlags.Instance.ClearAllFlags();

            if (loadedData.flagData != null && loadedData.flagData.activeFlags != null)
            {
                foreach (string flag in loadedData.flagData.activeFlags)
                    GameFlags.Instance.SetFlag(flag);

                Debug.Log($"[GameBootstrap] {loadedData.flagData.activeFlags.Length} 件のフラグを復元しました");
            }

            // セーブに存在しないトリガーはfalse扱い
            var allTriggers = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                .Where(x => x.gameObject.scene.isLoaded && x.GetType().Name.EndsWith("Trigger"));

            foreach (var obj in allTriggers)
            {
                string flagID = obj.GetType().Name;
                bool existsInSave = loadedData.flagData?.activeFlags?.Contains(flagID) ?? false;

                if (!existsInSave)
                {
                    GameFlags.Instance.RemoveFlag(flagID);
                    Debug.Log($"[GameBootstrap] {flagID} はセーブデータに存在しないため false 扱いにしました");
                }
            }
        }

        // ===== プレイヤー位置・向き =====
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            var playerMove = playerObj.GetComponent<GridMovement>();
            if (playerMove != null)
            {
                playerObj.transform.position = loadedData.playerPosition;
                playerMove.SetDirection(loadedData.playerDirection);
                Physics2D.SyncTransforms();
                Debug.Log($"[GameBootstrap] プレイヤー位置と向きを復元: {loadedData.playerPosition} / Dir={loadedData.playerDirection}");
            }
            else
            {
                Debug.LogWarning("[GameBootstrap] GridMovement が見つからず、向きの復元をスキップしました");
            }
        }
        else
        {
            Debug.LogWarning("[GameBootstrap] プレイヤーが見つからず、座標復元をスキップしました");
        }

        // ===== ギミック進行度 =====
        if (loadedData.gimmickProgressList != null && loadedData.gimmickProgressList.Count > 0)
        {
            var allGimmicks = Resources.FindObjectsOfTypeAll<GimmickBase>();
            foreach (var g in loadedData.gimmickProgressList)
            {
                var gimmick = allGimmicks.FirstOrDefault(x =>
                    x.gimmickID == g.gimmickID && x.gameObject.scene.isLoaded);

                if (gimmick != null)
                {
                    gimmick.LoadProgress(g.stage);
                    Debug.Log($"[GameBootstrap] ギミック {g.gimmickID} を Stage={g.stage} に復元しました");
                }
            }
        }

        // ===== アイテムトリガー =====
        if (loadedData.itemTriggerList != null && loadedData.itemTriggerList.Count > 0)
        {
            var allItemTriggers = Resources.FindObjectsOfTypeAll<ItemTrigger>();
            foreach (var i in loadedData.itemTriggerList)
            {
                var trigger = allItemTriggers.FirstOrDefault(x =>
                    x.triggerID == i.triggerID && x.gameObject.scene.isLoaded);

                if (trigger != null)
                {
                    trigger.LoadProgress(i.currentStage);
                    Debug.Log($"[GameBootstrap] アイテムトリガー {i.triggerID} を Stage={i.currentStage} に復元しました");
                }
            }
        }

        Debug.Log("[GameBootstrap] セーブデータ適用完了（シーン＋座標＋向きフル復元）");

        loadedData = null;
        Destroy(gameObject);
    }
}