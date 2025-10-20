using System.Collections;
using System.IO;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [Header("���̃g���K�[�ŗL��ID�iGameFlags�ɓo�^�j")]
    public string triggerID;

    [Header("�E����A�C�e���f�[�^")]
    public ItemData itemData;

    [Header("�����ڂ������I�u�W�F�N�g�i�C�Ӂj")]
    public GameObject targetObject;

    [Header("�K�v�Ȍ��� (0=��,1=��,2=�E,3=��, -1=�����Ȃ�)")]
    public int requiredDirection = -1;

    [Header("�E����������ׂ���H")]
    public bool canInspectAfterCollected = false;

    [Header("�V�X�e�����b�Z�[�W�t�@�C�����iStreamingAssets/SystemWindow�ȉ��j")]
    public string[] systemMessageFiles;

    [Header("�e�L�X�g�Đ����Ƀv���C���[���~���邩")]
    public bool freezeDuringText = true;

    private bool isPlayerNear = false;
    private GridMovement playerMovement;
    private int currentStage = 0;

    void Start()
    {
        // === �t���O�ɂ���ԕ��� ===
        if (GameFlags.Instance != null && GameFlags.Instance.HasFlag(triggerID))
        {
            currentStage = 1;
            UpdateVisualState();
        }
        else
        {
            currentStage = 0;
            UpdateVisualState();
        }
    }

    void Update()
    {
        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            if (requiredDirection != -1 && playerMovement != null &&
                playerMovement.GetDirection() != requiredDirection)
                return;

            if (currentStage == 0)
                CollectItem();
            else if (canInspectAfterCollected)
                StartCoroutine(PlaySystemTextRoutine());
        }
    }

    private void CollectItem()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            GameFlags.Instance?.SetFlag(triggerID);
            currentStage = 1;

            UpdateVisualState();
            StartCoroutine(PlaySystemTextRoutine());
        }
    }

    private IEnumerator PlaySystemTextRoutine()
    {
        if (freezeDuringText && playerMovement != null)
            playerMovement.enabled = false;

        foreach (var fileName in systemMessageFiles)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, "SystemWindow", fileName + ".txt");
            if (!File.Exists(fullPath)) continue;

            string text = File.ReadAllText(fullPath);
            DialogueCore.Instance?.StartConversation(fileName, text);
            yield return new WaitUntil(() => !IsConversationActive());
        }

        if (freezeDuringText && playerMovement != null)
            playerMovement.enabled = true;
    }

    private bool IsConversationActive()
    {
        var core = FindObjectOfType<DialogueCore>();
        return core != null && core.enabled;
    }

    private void UpdateVisualState()
    {
        if (targetObject == null) return;
        targetObject.SetActive(currentStage == 0 || canInspectAfterCollected);
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

    // ==========================================
    //  �Z�[�u�p�f�[�^�\���ƕ����p���\�b�h
    // ==========================================
    public ItemTriggerSaveData SaveProgress()
    {
        return new ItemTriggerSaveData
        {
            triggerID = this.triggerID,
            currentStage = this.currentStage
        };
    }

    public void LoadProgress(int stage)
    {
        this.currentStage = stage;
        UpdateVisualState();
    }
}

[System.Serializable]
public class ItemTriggerSaveData
{
    public string triggerID;
    public int currentStage;
}