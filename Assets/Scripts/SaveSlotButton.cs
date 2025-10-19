using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSlotButton : MonoBehaviour
{
    [Header("�X���b�g�ݒ�")]
    public int slotNumber = 1;

    [Header("UI�Q��")]
    [SerializeField] private TMP_Text slotLabel;

    [Header("�T�E���h�ݒ�")]
    [SerializeField] private AudioClip clickSeClip;

    private bool isLoadMode = false;

    private void Start()
    {
        UpdateLabel();
    }

    public void OnClickSlot()
    {
        // --- ���ʉ� ---
        if (SoundManager.Instance != null && clickSeClip != null)
            SoundManager.Instance.PlaySE(clickSeClip);

        if (isLoadMode)
        {
            // --- ���[�h ---
            var data = SaveSystem.LoadGame(slotNumber);
            if (data != null)
            {
                Debug.Log($"[SaveSlotButton] �X���b�g{slotNumber}�����[�h��...");
                GameBootstrap.loadedData = data;

                // Bootstrap���ێ������܂܃Q�[���V�[����Additive���[�h
                SceneManager.LoadSceneAsync(data.sceneName, LoadSceneMode.Additive);
            }
            else
            {
                Debug.LogWarning($"[SaveSlotButton] �X���b�g{slotNumber}�Ƀ��[�h�\�ȃf�[�^������܂���B");
            }
        }
        else
        {
            // --- �Z�[�u ---
            var player = FindFirstObjectByType<GridMovement>();
            if (player != null)
            {
                SaveSystem.SaveGame(slotNumber,
                    SceneManager.GetActiveScene().name,
                    player.transform.position,
                    player.GetDirection());
                Debug.Log($"[SaveSlotButton] �X���b�g{slotNumber}�ɃZ�[�u���܂����B");
            }
        }

        // ����
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
        string modeText = isLoadMode ? "���[�h" : "�Z�[�u";
        slotLabel.text = $"�X���b�g {slotNumber} �i{modeText}�j";
    }
}