using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonSequenceGimmick : GimmickBase
{
    [Header("�M�~�b�N�ݒ�")]
    public ItemData rewardItemData;

    [Header("����/���Z�b�g���ɕ\�������bID")]
    public string initialConversationId;

    [Header("�����̃{�^������ (�C���f�b�N�X 0�`3)")]
    [Tooltip("�N���b�N���ׂ��{�^���̃C���f�b�N�X�����Ԃɐݒ�")]
    public List<int> correctSequence = new List<int> { 0, 1, 2, 3 };

    private List<int> inputSequence = new List<int>();
    private GimmickCanvasController canvasController;

    private bool initialized = false;

    // -----------------------------------------------------
    private void Start()
    {
        StartCoroutine(WaitForBootstrapAndCanvas());
    }

    private System.Collections.IEnumerator WaitForBootstrapAndCanvas()
    {
        yield return new WaitUntil(() =>
            BootLoader.HasBooted &&
            GimmickCanvasController.Instance != null);

        canvasController = GimmickCanvasController.Instance;
        InitializeGimmick();
        initialized = true;
    }

    private void InitializeGimmick()
    {
        if (currentStage == 0)
        {
            inputSequence.Clear();
            DisplayConversationPage(initialConversationId);
        }
        else if (currentStage < correctSequence.Count)
        {
            inputSequence = Enumerable.Repeat(0, currentStage).ToList();
            DisplayConversationPage($"{initialConversationId}_{currentStage}");
        }
        else
        {
            inputSequence = Enumerable.Repeat(0, correctSequence.Count).ToList();
            DisplayConversationPage($"{initialConversationId}_Completed");
        }
    }

    // -----------------------------------------------------
    // UI����p�֐��i�{�^���������ɌĂ΂��j
    public void OnButtonClick(int clickedIndex)
    {
        if (!initialized) return;
        if (currentStage >= correctSequence.Count) return; // �����ς�

        int expected = correctSequence[currentStage];
        if (clickedIndex == expected)
        {
            inputSequence.Add(clickedIndex);
            currentStage++;

            if (currentStage == correctSequence.Count)
            {
                CompleteGimmick();
                return;
            }

            Debug.Log($"[Sequence] ����: {currentStage}/{correctSequence.Count}");
            DisplayConversationPage($"{initialConversationId}_{currentStage}");
        }
        else
        {
            ResetSequence();
        }
    }

    // -----------------------------------------------------
    private void ResetSequence()
    {
        inputSequence.Clear();
        currentStage = 0;
        Debug.Log("[Sequence] �s���� �� ���Z�b�g");
        DisplayConversationPage(initialConversationId);
    }

    private void CompleteGimmick()
    {
        Debug.Log("[Sequence] �M�~�b�N�����I");

        if (rewardItemData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(rewardItemData);
            Debug.Log($"[Sequence] ��V {rewardItemData.itemName} �����I");
        }

        DisplayConversationPage($"{initialConversationId}_Completed");
        canvasController?.HideCanvas();
    }

    // -----------------------------------------------------
    // �Z�[�u�E���[�h
    public override GimmickSaveData SaveProgress()
    {
        var data = base.SaveProgress();
        data.stage = currentStage;
        return data;
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);
        InitializeGimmick();
    }

    // -----------------------------------------------------
    // ��b�Ăяo��
    public void DisplayConversationPage(string conversationId)
    {
        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire(conversationId);
        }
        else
        {
            Debug.LogWarning($"[Sequence] ConversationHub��������܂���BID: {conversationId}");
        }
    }
}
