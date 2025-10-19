using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotButton : MonoBehaviour
{
    [Header("スロット設定")]
    public int slotNumber = 1;

    [Header("UI参照")]
    [SerializeField] private TMP_Text slotLabel;

    [Header("サウンド設定")]
    [SerializeField] private AudioClip clickSeClip;

    private bool isLoadMode = false;

    private void Start()
    {
        UpdateLabel();
    }

    public void OnClickSlot()
    {
        // --- 効果音 ---
        if (SoundManager.Instance != null && clickSeClip != null)
            SoundManager.Instance.PlaySE(clickSeClip);

        if (isLoadMode)
        {
            // ==========================
            //  ロードモード処理
            // ==========================
            var data = SaveSystem.LoadGame(slotNumber);
            if (data != null)
            {
                Debug.Log($"[SaveSlotButton] スロット{slotNumber}ロード開始：{data.sceneName}");

                // BootLoader取得
                var boot = FindFirstObjectByType<BootLoader>();
                if (boot == null)
                {
                    Debug.LogError("[SaveSlotButton] BootLoaderが見つかりません！");
                    return;
                }

                // --- すべてのシーンをOFFにして対象シーンだけON ---
                foreach (var kv in boot.loadedScenes)
                    boot.SetSceneActive(kv.Key, kv.Key == data.sceneName);

                // --- アクティブシーンを切り替え ---
                var targetScene = SceneManager.GetSceneByName(data.sceneName);
                if (targetScene.IsValid())
                {
                    SceneManager.SetActiveScene(targetScene);
                    Debug.Log($"[SaveSlotButton] アクティブシーンを {data.sceneName} に変更しました。");
                }

                // --- GameBootstrapを動的生成してロード適用 ---
                GameBootstrap.loadedData = data;
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();

                Debug.Log($"[SaveSlotButton] BootLoader経由で {data.sceneName} のロードを開始しました。");
            }
            else
            {
                Debug.LogWarning($"[SaveSlotButton] スロット{slotNumber}にロード可能なデータがありません。");
            }
        }
        else
        {
            // ==========================
            //  セーブモード処理
            // ==========================
            var player = FindFirstObjectByType<GridMovement>();
            if (player != null)
            {
                SaveSystem.SaveGame(
                    slotNumber,
                    SceneManager.GetActiveScene().name,
                    player.transform.position,
                    player.GetDirection()
                );
                Debug.Log($"[SaveSlotButton] スロット{slotNumber}にセーブしました。");
            }
            else
            {
                Debug.LogWarning("[SaveSlotButton] プレイヤーが見つかりません。セーブ失敗。");
            }
        }

        // --- UIを閉じる ---
        if (SaveSlotUIManager.Instance != null)
            SaveSlotUIManager.Instance.ClosePanel();

        UpdateLabel();
    }

    public void SetMode(bool loadMode)
    {
        isLoadMode = loadMode;
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (slotLabel == null) return;
        string modeText = isLoadMode ? "ロード" : "セーブ";
        slotLabel.text = $"スロット {slotNumber} （{modeText}）";
    }
}