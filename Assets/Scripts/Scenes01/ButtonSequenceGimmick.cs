using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// �{�^���̏����N���b�N�ƃe�L�X�g�\���𐧌䂷��M�~�b�N
public class ButtonSequenceGimmick : GimmickBase
{
    [Header("�M�~�b�N�ݒ�")]
    public ItemData rewardItemData;

    [Header("����/���Z�b�g���ɕ\�������bID")]
    public string initialConversationId;

    [Header("�����̃{�^������ (�C���f�b�N�X 0�`3)")]
    [Tooltip("�N���b�N���ׂ��{�^���̃C���f�b�N�X�����Ԃɐݒ�")]
    public List<int> correctSequence = new List<int> { 0, 1, 2, 3 }; // ��: �{�^��1��2��3��4

    // currentStage �Ɍ��݂̓��̓X�e�b�v��ۑ�����
    private List<int> inputSequence = new List<int>();

    // �Q��
    private GimmickCanvasController canvasController;

    // -----------------------------------------------------

    void Start()
    {
        // GimmickCanvasController���V�[�������猟��
        canvasController = FindObjectOfType<GimmickCanvasController>();

        if (currentStage == 0)
        {
            inputSequence.Clear();
            // �����e�L�X�g��\��
            DisplayConversationPage(initialConversationId);
        }
        else
        {
            // ���[�h�܂��� Stage 1 �ȏ�ŊJ�n���ꂽ�ꍇ�A������Ԃ̂܂܂ɂ���
            inputSequence = Enumerable.Repeat(0, correctSequence.Count).ToList();
            // ���������e�L�X�g��\��
            DisplayConversationPage($"{initialConversationId}_Completed");
        }
    }

    /// <summary>
    /// GimmickCanvas�̃{�^�����N���b�N���ꂽ�Ƃ��ɌĂяo�����
    /// </summary>
    /// <param name="clickedIndex">�N���b�N���ꂽ�{�^���̃C���f�b�N�X (0�`3)</param>
    public void OnButtonClick(int clickedIndex)
    {
        if (currentStage >= correctSequence.Count) return; // ���Ɋ����ς�

        if (inputSequence.Count == currentStage)
        {
            // ���̓��̓X�e�b�v���`�F�b�N
            int expectedIndex = correctSequence[currentStage];

            if (clickedIndex == expectedIndex)
            {
                // --- �������� ---
                inputSequence.Add(clickedIndex);
                this.currentStage++; // �X�e�[�W�i�s�i�Z�[�u�Ώہj

                if (currentStage == correctSequence.Count)
                {
                    // �M�~�b�N�����I
                    CompleteGimmick();
                    return;
                }

                // ���̃e�L�X�g��\�� (�X�e�b�v���ɑΉ�����ID��z��)
                Debug.Log($"[Sequence] �����B���̃X�e�b�v�� ({currentStage}/{correctSequence.Count})");
                DisplayConversationPage($"{initialConversationId}_{currentStage}");
            }
            else
            {
                // --- �s�������� ---
                inputSequence.Clear();
                this.currentStage = 0;
                Debug.Log($"[Sequence] �s�����B�V�[�P���X�����Z�b�g���܂����B");
                DisplayConversationPage(initialConversationId); // �ŏ��̃e�L�X�g�ɖ߂�
            }
        }
    }

    /// <summary>
    /// �M�~�b�N�����������Ƃ��̍ŏI����
    /// </summary>
    private void CompleteGimmick()
    {
        if (rewardItemData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(rewardItemData);
            Debug.Log($"[Sequence] �M�~�b�N�����I��V {rewardItemData.itemName} ����肵�܂����B");
        }

        // ������̃e�L�X�g��\��
        DisplayConversationPage($"{initialConversationId}_Completed");

        // Canvas�����
        if (canvasController != null)
        {
            canvasController.HideCanvas();
        }

        // GimmickBase��Complete���\�b�h�������ŌĂ�ł��ǂ����A����͊�����Ԃ�currentStage�ŊǗ�
    }

    /// <summary>
    /// DialogueCore�̋@�\���g���ăe�L�X�g�p�l���ɉ�b��\������
    /// </summary>
    /// <param name="conversationId">�\�������bID</param>
    public void DisplayConversationPage(string conversationId)
    {
        // �����̉�b�V�X�e���iConversationHub�j���N������
        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire(conversationId);
        }
        else
        {
            Debug.LogWarning($"[Sequence] ConversationHub��������܂���B��bID: {conversationId} �͕\������܂���B");
        }
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        // ���[�h��̕�������
        inputSequence.Clear();
        if (currentStage > 0)
        {
            // �����ς݂܂��͓r���܂Ői��ł�����Ԃ��Č�
            inputSequence = Enumerable.Repeat(0, currentStage).ToList();
        }

        // ������Ԃł���΁A������̃e�L�X�g��\��
        if (currentStage >= correctSequence.Count)
        {
            DisplayConversationPage($"{initialConversationId}_Completed");
        }
        else
        {
            // �r���i�K�܂��͏�����Ԃ̃e�L�X�g��\��
            DisplayConversationPage($"{initialConversationId}_{currentStage}");
        }
    }
}