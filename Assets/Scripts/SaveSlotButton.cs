using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SaveSlotButton : MonoBehaviour
{
    [Header("�X���b�g�ݒ�")]
    public int slotNumber = 1;

    [Header("UI�Q��")]
    [SerializeField] private TMP_Text slotLabel;
    [SerializeField] private TMP_Text detailLabel; // �� �ǉ��F�v���C���Ԃ�V�[�����\���p

    [Header("�T�E���h�ݒ�")]
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
            Debug.Log($"[SaveSlotButton] �X���b�g{slotNumber}�͉{����p���[�h�̂��ߖ����B");
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
                    Debug.LogError("[SaveSlotButton] BootLoader��������܂���I");
                    return;
                }

                foreach (var kv in boot.loadedScenes)
                    boot.SetSceneActive(kv.Key, kv.Key == data.sceneName);

                var targetScene = SceneManager.GetSceneByName(data.sceneName);
                if (targetScene.IsValid())
                    SceneManager.SetActiveScene(targetScene);

                GameBootstrap.loadedData = data;
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();

                PlayerPrefs.SetInt("LastUsedSlot", slotNumber); // �����[�h�����L�^
            }
            else
            {
                Debug.LogWarning($"[SaveSlotButton] �X���b�g{slotNumber}�Ƀ��[�h�\�ȃf�[�^������܂���B");
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
                Debug.Log($"[SaveSlotButton] �X���b�g{slotNumber}�ɃZ�[�u���܂����B");

                PlayerPrefs.SetInt("LastUsedSlot", slotNumber); // ���Ō�Ɏg�����X���b�g���L�^
            }
            else
            {
                Debug.LogWarning("[SaveSlotButton] �v���C���[��������܂���B�Z�[�u���s�B");
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

        string modeText = isViewOnly ? "�{��" :
                          isLoadMode ? "���[�h" :
                          "�Z�[�u";

        // --- �Z�[�u�f�[�^���ǂݍ��� ---
        var data = SaveSystem.LoadGame(slotNumber);

        if (data != null)
        {
            string sceneName = data.sceneName;
            TimeSpan playTime = TimeSpan.FromSeconds(data.playtime);
            string timeText = $"{(int)playTime.TotalHours:D2}:{playTime.Minutes:D2}:{playTime.Seconds:D2}";
            slotLabel.text = $"�X���b�g {slotNumber} �i{modeText}�j";
            if (detailLabel != null)
                detailLabel.text = $"{sceneName}\n{timeText}";
        }
        else
        {
            slotLabel.text = $"�X���b�g {slotNumber} �i{modeText}�j";
            if (detailLabel != null)
                detailLabel.text = "NO DATA";
        }
    }
}
