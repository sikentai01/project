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
            // --- ロード ---
            var data = SaveSystem.LoadGame(slotNumber);
            if (data != null)
            {
                Debug.Log($"[SaveSlotButton] スロット{slotNumber}をロード中...");
                GameBootstrap.loadedData = data;

                // Bootstrapを維持したままゲームシーンをAdditiveロード
                SceneManager.LoadSceneAsync(data.sceneName, LoadSceneMode.Additive);
            }
            else
            {
                Debug.LogWarning($"[SaveSlotButton] スロット{slotNumber}にロード可能なデータがありません。");
            }
        }
        else
        {
            // --- セーブ ---
            var player = FindFirstObjectByType<GridMovement>();
            if (player != null)
            {
                SaveSystem.SaveGame(slotNumber,
                    SceneManager.GetActiveScene().name,
                    player.transform.position,
                    player.GetDirection());
                Debug.Log($"[SaveSlotButton] スロット{slotNumber}にセーブしました。");
            }
        }

        // 閉じる
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