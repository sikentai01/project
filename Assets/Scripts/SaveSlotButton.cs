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
            // ==========================
            //  ���[�h���[�h����
            // ==========================
            var data = SaveSystem.LoadGame(slotNumber);
            if (data != null)
            {
                Debug.Log($"[SaveSlotButton] �X���b�g{slotNumber}���[�h�J�n�F{data.sceneName}");

                // BootLoader�擾
                var boot = FindFirstObjectByType<BootLoader>();
                if (boot == null)
                {
                    Debug.LogError("[SaveSlotButton] BootLoader��������܂���I");
                    return;
                }

                // --- ���ׂẴV�[����OFF�ɂ��đΏۃV�[������ON ---
                foreach (var kv in boot.loadedScenes)
                    boot.SetSceneActive(kv.Key, kv.Key == data.sceneName);

                // --- �A�N�e�B�u�V�[����؂�ւ� ---
                var targetScene = SceneManager.GetSceneByName(data.sceneName);
                if (targetScene.IsValid())
                {
                    SceneManager.SetActiveScene(targetScene);
                    Debug.Log($"[SaveSlotButton] �A�N�e�B�u�V�[���� {data.sceneName} �ɕύX���܂����B");
                }

                // --- GameBootstrap�𓮓I�������ă��[�h�K�p ---
                GameBootstrap.loadedData = data;
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();

                Debug.Log($"[SaveSlotButton] BootLoader�o�R�� {data.sceneName} �̃��[�h���J�n���܂����B");
            }
            else
            {
                Debug.LogWarning($"[SaveSlotButton] �X���b�g{slotNumber}�Ƀ��[�h�\�ȃf�[�^������܂���B");
            }
        }
        else
        {
            // ==========================
            //  �Z�[�u���[�h����
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
                Debug.Log($"[SaveSlotButton] �X���b�g{slotNumber}�ɃZ�[�u���܂����B");
            }
            else
            {
                Debug.LogWarning("[SaveSlotButton] �v���C���[��������܂���B�Z�[�u���s�B");
            }
        }

        // --- UI����� ---
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