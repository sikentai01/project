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
        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        // �v���C���[���߂��ɂ��� Enter�L�[ �������ꂽ��Z�[�u�X���b�gUI���J��
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // ��������
            int playerDir = player.GetDirection();
            if (requiredDirection == -1 || playerDir == requiredDirection)
            {
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
            isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }
}