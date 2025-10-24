using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ButtonSequenceGimmick : GimmickBase
{
    [Header("�M�~�b�N�ݒ�")]
    public ItemData rewardItemData;

    [Header("����/���Z�b�g���ɕ\�������bID")]
    public string initialConversationId;

    [Header("�����̃{�^������ (�C���f�b�N�X 0�`3)")]
    public List<int> correctSequence = new List<int> { 0, 1, 2, 3 };

    private List<int> inputSequence = new List<int>();
    private GimmickCanvasController canvasController;

    private bool isPlayerNear = false;
    private bool isActive = false; // ���݃L�����o�X���쒆���ǂ���

    // =====================================================
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => BootLoader.HasBooted);

        canvasController = GimmickCanvasController.Instance;
        inputSequence.Clear();

        if (currentStage > 0)
            inputSequence = Enumerable.Repeat(0, currentStage).ToList();

        Debug.Log($"[Gimmick] {gimmickID} ���������� stage={currentStage}");
    }


    private void Update()
    {
        if (!isPlayerNear) return;
        if (PauseMenu.isPaused) return;
        if (BootLoader.IsPlayerSpawning) return;

        // === Enter�L�[���� ===
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isActive)
            {
                OpenCanvas();
            }
        }
    }

    private void OpenCanvas()
    {
        if (canvasController == null)
        {
            Debug.LogWarning("[ButtonSequenceGimmick] CanvasController ��������܂���B");
            return;
        }

        isActive = true;
        canvasController.ShowCanvas(this);
        DisplayConversationPage(initialConversationId);
    }

    // =====================================================
    //  �{�^�������C�x���g�iCanvasController����Ă΂��j
    // =====================================================
    public void OnButtonClick(int clickedIndex)
    {
        if (currentStage >= correctSequence.Count)
        {
            CloseCanvas();
            return;
        }

        if (inputSequence.Count == currentStage)
        {
            int expectedIndex = correctSequence[currentStage];

            if (clickedIndex == expectedIndex)
            {
                inputSequence.Add(clickedIndex);
                currentStage++;

                if (currentStage == correctSequence.Count)
                {
                    CompleteGimmick();
                    return;
                }

                Debug.Log($"[Sequence] �����B���̃X�e�b�v�� ({currentStage}/{correctSequence.Count})");
                DisplayConversationPage($"{initialConversationId}_{currentStage}");
            }
            else
            {
                // �s���� �� ���Z�b�g
                inputSequence.Clear();
                currentStage = 0;
                Debug.Log($"[Sequence] �s�����B���Z�b�g�B");
                DisplayConversationPage(initialConversationId);
            }
        }
    }

    private void CompleteGimmick()
    {
        if (rewardItemData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(rewardItemData);
            Debug.Log($"[Sequence] �M�~�b�N�����I��V {rewardItemData.itemName} ����肵�܂����B");
        }

        DisplayConversationPage($"{initialConversationId}_Completed");
        CloseCanvas();
    }

    private void CloseCanvas()
    {
        if (canvasController != null)
            canvasController.HideCanvas();

        isActive = false;
    }

    // =====================================================
    //  ��b�\���iDialogueCore / ConversationHub �o�R�j
    // =====================================================
    private void DisplayConversationPage(string conversationId)
    {
        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire(conversationId);
        }
        else
        {
            Debug.LogWarning($"[Sequence] ConversationHub ��������܂���B��bID: {conversationId}");
        }
    }

    // =====================================================
    //  �v���C���[�ڋߔ���i�M�~�b�N���g�ŊǗ��j
    // =====================================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (isActive)
                CloseCanvas();
        }
    }

    // =====================================================
    //  �Z�[�u / ���[�h�i���ʉ��j
    // =====================================================
    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        inputSequence.Clear();
        if (stage > 0)
            inputSequence = Enumerable.Repeat(0, stage).ToList();

        Debug.Log($"[Gimmick] {gimmickID} LoadProgress: {stage}");
    }
}
