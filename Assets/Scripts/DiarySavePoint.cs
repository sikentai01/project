using UnityEngine;

public class DiarySavePoint : MonoBehaviour
{
    [Header("�T�E���h�ݒ�")]
    [SerializeField] private AudioClip saveSeClip;

    [Header("���o")]
    [SerializeField] private GameObject saveEffect;  // ����G�t�F�N�g�Ȃǁi�C�Ӂj

    [Header("�����w��")]
    [Tooltip("0=��, 1=��, 2=�E, 3=��, -1=�ǂ̌����ł�OK")]
    [SerializeField] private int requiredDirection = -1;

    [Header("�C�x���g��p���L���ǂ���")]
    [Tooltip("�C�x���g��ɂ����Z�[�u�ł��Ȃ����L�Ȃ�`�F�b�N������")]
    [SerializeField] private bool requiresUnlockFlag = false;

    private bool isPlayerNear = false;
    private GridMovement player;

    void Start()
    {
        player = FindFirstObjectByType<GridMovement>();
        if (saveEffect != null)
            saveEffect.SetActive(false);
    }

    void Update()
    {
        // BootLoader��Additive�Ή�
        if (player == null)
        {
            player = FindFirstObjectByType<GridMovement>();
            if (player == null)
                return;
        }

        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            int playerDir = player.GetDirection();
            if (requiredDirection == -1 || playerDir == requiredDirection)
            {
                // --- �t���O�m�F ---
                if (requiresUnlockFlag)
                {
                    if (GameFlags.Instance == null || !GameFlags.Instance.HasFlag("DiaryEventUnlocked"))
                    {
                        Debug.Log("[DiarySavePoint] ���̓��L�͂܂��L�^�ł��Ȃ��B�C�x���g���K�v�ł��B");
                        // �C�ӂŃV�X�e�����b�Z�[�W�ȂǕ\����
                        // SystemMessage.Show("�܂��L�^�ł��Ȃ��悤���c");
                        return;
                    }
                }

                OpenSaveUI();
            }
            else
            {
                Debug.Log("�v���C���[�̌����������Ă��Ȃ����߃Z�[�u�ł��܂���B");
            }
        }
    }

    private void OpenSaveUI()
    {
        // ���ʉ��Đ�
        if (SoundManager.Instance != null && saveSeClip != null)
            SoundManager.Instance.PlaySE(saveSeClip);

        // ���o
        if (saveEffect != null)
        {
            saveEffect.SetActive(true);
            Invoke(nameof(StopEffect), 1.0f);
        }

        // �v���C���[�̓������~�߂�
        if (player != null)
            player.enabled = false;

        // �Z�[�u�X���b�gUI���J��
        if (SaveSlotUIManager.Instance != null)
            SaveSlotUIManager.Instance.OpenSavePanel();
    }

    private void StopEffect()
    {
        if (saveEffect != null)
            saveEffect.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            player = other.GetComponent<GridMovement>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}
