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

    private bool isPlayerNear = false;
    private GridMovement playerMovement;

    public bool IsPlayerNear => isPlayerNear;

    private void Start()
    {
        // --- ���[�h���Ɍ����ڂ𕜌� ---
        if (currentStage == 0)
        {
            if (targetObject != null)
                targetObject.SetActive(true);
        }
        else
        {
            if (targetObject != null)
                targetObject.SetActive(false);
        }
    }

    void Update()
    {
        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // �����`�F�b�N
            if (requiredDirection != -1 && playerMovement != null && playerMovement.GetDirection() != requiredDirection)
            {
                Debug.Log("�����������Ă��Ȃ��̂ŃA�C�e�����擾�ł��Ȃ�");
                return;
            }

            // ���擾�̂Ƃ��̂ݏE����
            if (currentStage == 0)
                CollectItem();
        }
    }

    // --- �A�C�e�����菈�� ---
    public void CollectItem()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            Debug.Log(itemData.itemName + " �����I");

            currentStage = 1; // �E�����̂Ői�s�x�X�V

            if (targetObject != null)
                targetObject.SetActive(false);
            else
                gameObject.SetActive(false);

            // �Z�[�u�A������Ȃ炱����GameFlags�ɓo�^����
            GameFlags.Instance?.SetFlag(triggerID);
        }
    }

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

    // =======================
    //  �Z�[�u�^���[�h�Ή��ǉ�
    // =======================
    public SaveSystem.GimmickProgressData SaveProgress()
    {
        return new SaveSystem.GimmickProgressData
        {
            triggerID = this.triggerID,
            stage = this.currentStage
        };
    }

    public void LoadProgress(int savedStage)
    {
        currentStage = savedStage;

        // --- �����ڂ�i�s�x�ɍ��킹�ĕ��� ---
        if (currentStage == 0)
        {
            if (targetObject != null)
                targetObject.SetActive(true);
        }
        else
        {
            if (targetObject != null)
                targetObject.SetActive(false);
        }

        Debug.Log($"[ItemTrigger] {triggerID} �̐i�s�x�� {currentStage} �ɕ������܂����B");
    }
}