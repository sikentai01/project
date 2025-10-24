using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SaveSlotButton : MonoBehaviour
{
    [Header("スロット設定")]
    public int slotNumber = 1;

    [Header("UI参照")]
    [SerializeField] private TMP_Text slotLabel;
    [SerializeField] private TMP_Text detailLabel; // ← 追加：プレイ時間やシーン名表示用

    [Header("サウンド設定")]
    [SerializeField] private AudioClip clickSeClip;

    private bool isLoadMode = false;
    private bool isViewOnly = false;

    private void Start()
    {
        UpdateLabel();
    }

    public void OnClickSlot()
    {
        if (SoundManager.Instance != null && clickSeClip != null)
            SoundManager.Instance.PlaySE(clickSeClip);

        if (isViewOnly)
        {
            Debug.Log($"[SaveSlotButton] スロット{slotNumber}は閲覧専用モードのため無効。");
            return;
        }

        if (isLoadMode)
        {
            var data = SaveSystem.LoadGame(slotNumber);
            if (data != null)
            {
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
                    SceneManager.SetActiveScene(targetScene);

                GameBootstrap.loadedData = data;
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();

                PlayerPrefs.SetInt("LastUsedSlot", slotNumber); // ★ロード時も記録
            }
            else
            {
                Debug.LogWarning($"[SaveSlotButton] スロット{slotNumber}にロード可能なデータがありません。");
            }
        }
        else
        {
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

                PlayerPrefs.SetInt("LastUsedSlot", slotNumber); // ★最後に使ったスロットを記録
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

        // --- セーブデータ情報読み込み ---
        var data = SaveSystem.LoadGame(slotNumber);

        if (data != null)
        {
            string sceneName = data.sceneName;
            TimeSpan playTime = TimeSpan.FromSeconds(data.playtime);
            string timeText = $"{(int)playTime.TotalHours:D2}:{playTime.Minutes:D2}:{playTime.Seconds:D2}";
            slotLabel.text = $"スロット {slotNumber} （{modeText}）";
            if (detailLabel != null)
                detailLabel.text = $"{sceneName}\n{timeText}";
        }
        else
        {
            slotLabel.text = $"スロット {slotNumber} （{modeText}）";
            if (detailLabel != null)
                detailLabel.text = "NO DATA";
        }
    }
}
