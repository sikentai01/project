using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [Header("���̃g���K�[�ŗL��ID�i�Z�[�u�p�Ƀ��j�[�N�ɐݒ�j")]
    public string triggerID;

    [Header("�E����A�C�e���f�[�^")]
    public ItemData itemData;

    [Header("�����ڂ������I�u�W�F�N�g�i�C�Ӂj")]
    public GameObject targetObject;

    [Header("�K�v�Ȍ��� (0=��,1=��,2=�E,3=��, -1=�����Ȃ�)")]
    public int requiredDirection = -1;

    [Header("���݂̐i�s�x (0=���擾, 1=�擾��)")]
    public int currentStage = 0;

    [Header("�E�������ɍĐ�����T�E���h�i�C�Ӂj")]
    public AudioClip pickupSE;

    private bool isPlayerNear = false;
    private GridMovement playerMovement;

    public bool IsPlayerNear => isPlayerNear;

    private void Start()
    {
        UpdateVisual();
    }

    private void Update()
    {
        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // �����`�F�b�N
            if (requiredDirection != -1 && playerMovement != null &&
                playerMovement.GetDirection() != requiredDirection)
                return;

            // ���擾�̂Ƃ��̂ݏE����
            if (currentStage == 0)
                CollectItem();
        }
    }

    // === �A�C�e�����菈�� ===
    private void CollectItem()
    {
        if (itemData == null) return;

        // �@ �A�C�e���ǉ�
        InventoryManager.Instance.AddItem(itemData);
        Debug.Log($"[ItemTrigger] {itemData.itemName} �����I");

        // �A ��ԍX�V
        currentStage = 1;
        UpdateVisual();

        // �B ���ʉ��Đ��i�C�Ӂj
        if (SoundManager.Instance != null && pickupSE != null)
            SoundManager.Instance.PlaySE(pickupSE);

        // �C �A�j���[�V�����ʒm�iGameFlags�͎g��Ȃ��j
        var reactions = FindObjectsByType<GimmickReactionTarget>(FindObjectsSortMode.None);
        foreach (var r in reactions)
        {
            r.ReactToTrigger(triggerID);
        }

        Debug.Log($"[ItemTrigger] {triggerID} �����B�i�s�x={currentStage}");
    }

    // === �����ڂ�؂�ւ� ===
    private void UpdateVisual()
    {
        if (targetObject != null)
            targetObject.SetActive(currentStage == 0);
    }

    // === �Z�[�u�p�f�[�^�쐬 ===
    public SaveSystem.GimmickProgressData SaveProgress()
    {
        return new SaveSystem.GimmickProgressData
        {
            triggerID = triggerID,
            stage = currentStage
        };
    }

    // === ���[�h���ɐi�s�x�𔽉f ===
    public void LoadProgress(int stage)
    {
        currentStage = stage;
        UpdateVisual();
        Debug.Log($"[ItemTrigger] {triggerID} �̐i�s�x�� {currentStage} �ɕ������܂���");
    }

    // === �v���C���[�ڐG���� ===
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            playerMovement = other.GetComponent<GridMovement>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            playerMovement = null;
        }
    }
}