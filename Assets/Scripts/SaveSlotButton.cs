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
    private bool isViewOnly = false; //  追加

    private void Start()
    {
        UpdateLabel();
    }

    public void OnClickSlot()
    {
        // --- 効果音 ---
        if (SoundManager.Instance != null && clickSeClip != null)
            SoundManager.Instance.PlaySE(clickSeClip);

        //  閲覧専用モードでは何もしない
        if (isViewOnly)
        {
            Debug.Log($"[SaveSlotButton] スロット{slotNumber}は閲覧専用モードのため無効。");
            return;
        }

        if (isLoadMode)
        {
            // ==========================
            //  ロードモード処理
            // ==========================
            var data = SaveSystem.LoadGame(slotNumber);
            if (data != null)
            {
                Debug.Log($"[SaveSlotButton] スロット{slotNumber}ロード開始：{data.sceneName}");

                var boot = FindFirstObjectByType<BootLoader>();
                if (boot == null)
                {
                    Debug.LogError("[SaveSlotButton] BootLoaderが見つかりません！");
                    return;
                }

                foreach (var kv in boot.loadedScenes)
                    boot.SetSceneActive(kv.Key, kv.Key == data.sceneName);

                var targetScene = SceneManager.GetSceneByName(data.sceneName);
                if (targetScene.IsValid())
                {
                    SceneManager.SetActiveScene(targetScene);
                    Debug.Log($"[SaveSlotButton] アクティブシーンを {data.sceneName} に変更しました。");
                }

                GameBootstrap.loadedData = data;
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
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

        if (SaveSlotUIManager.Instance != null)
            SaveSlotUIManager.Instance.ClosePanel();

        UpdateLabel();
    }

    // ======================================================
    //  モード設定（ビュー対応）
    // ======================================================
    public void SetMode(bool loadMode, bool viewOnly = false)
    {
        isLoadMode = loadMode;
        isViewOnly = viewOnly;
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (slotLabel == null) return;

        string modeText = isViewOnly ? "閲覧" :
                          isLoadMode ? "ロード" :
                          "セーブ";

        slotLabel.text = $"スロット {slotNumber} （{modeText}）";
    }
}
