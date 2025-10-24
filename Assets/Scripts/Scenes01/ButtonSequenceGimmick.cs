using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

/// <summary>
/// �{�^���̏����N���b�N�ƃe�L�X�g�\���𐧌䂷��M�~�b�N
/// </summary>
public class ButtonSequenceGimmick : GimmickBase
{
    [Header("����/���Z�b�g���ɕ\�����郁�b�Z�[�W")]
    public string initialMessage = "�����������Ń{�^���������Ă��������B";

    [Header("�����̃{�^������ (�C���f�b�N�X 0�`3)")]
    [Tooltip("�N���b�N���ׂ��{�^���̃C���f�b�N�X�����Ԃɐݒ�")]
    public List<int> correctSequence = new List<int> { 0, 1, 2, 3 };

    // UI�A�g�t�B�[���h
    [Header("UI�֘A")]
    public GameObject gimmickCanvasRoot;

    private bool isPlayerNear = false;
    private GimmickCanvasController canvasController;

    // currentStage �̓M�~�b�N�̐i�s�x
    private List<int> inputSequence = new List<int>();

    void Awake()
    {
        canvasController = FindObjectOfType<GimmickCanvasController>();
    }

    void Start()
    {
        // currentStage��0�̏ꍇ�A�C���v�b�g�V�[�P���X���N���A
        if (currentStage <= correctSequence.Count)
        {
            inputSequence.Clear();
        }

        // �N���ς� (currentStage > 0) �̏ꍇ�ACanvas���\���ɂ��Ă���
        if (currentStage > 0 && canvasController != null)
        {
            HideGimmickCanvas();
        }
    }

    // =====================================================
    // ������ �M�~�b�N�N�� (GimmickBase�̃��\�b�h�ł͂Ȃ�) ������
    // =====================================================

    /// <summary>
    /// Enter�L�[����ȂǂŁA�M�~�b�N���N������ (override���폜)
    /// </summary>
    public void StartSequence() // �� StartGimmick���疼�̕ύX
    {
        if (currentStage >= correctSequence.Count + 1)
        {
            DisplayMessage("�M�~�b�N�͊��ɉ�������Ă��܂��B");
            return;
        }

        // --- ����N��/���Z�b�g���̏��� ---
        if (currentStage < 1)
        {
            this.currentStage = 1; // �N����Ԃɂ��� (Stage 1)
            inputSequence.Clear();
            DisplayMessage(initialMessage);
        }
        else
        {
            // ���ɋN����Ԃ̏ꍇ�A���݂̃X�e�b�v�̃q���g���ĕ\��
            DisplayMessage($"�X�e�b�v {inputSequence.Count + 1} ����͂��Ă��������B");
        }

        ShowGimmickCanvas();
    }

    // ... (OnTriggerEnter/Exit2D, Update, OnButtonClick �͏ȗ�) ...

    // GimmickCanvasController���{�^���N���b�N�ŌĂяo�����\�b�h
    public void OnButtonClick(int clickedIndex)
    {
        if (currentStage < 1) return;
        if (currentStage >= correctSequence.Count + 1) return;

        int currentStep = inputSequence.Count;

        if (currentStep < correctSequence.Count)
        {
            int expectedIndex = correctSequence[currentStep];

            if (clickedIndex == expectedIndex)
            {
                // ��������
                inputSequence.Add(clickedIndex);
                this.currentStage++;

                if (this.currentStage >= correctSequence.Count + 1)
                {
                    CompleteGimmick();
                    return;
                }

                // ���̃e�L�X�g��\��
                Debug.Log($"[Sequence] �����B���̃X�e�b�v�� ({inputSequence.Count}/{correctSequence.Count})");
                DisplayMessage($"�����I�X�e�b�v {inputSequence.Count + 1} ����͂��Ă��������B");
            }
            else
            {
                // �s��������
                inputSequence.Clear();
                this.currentStage = 1;
                Debug.Log($"[Sequence] �s�����B�V�[�P���X�����Z�b�g���܂����B");
                DisplayMessage("�s�����ł��B�ŏ������蒼���Ă��������B");
            }
        }
    }


    private void CompleteGimmick()
    {
        DisplayMessage("�M�~�b�N���������I");

        if (canvasController != null)
        {
            canvasController.HideCanvas();
        }

        this.currentStage = correctSequence.Count + 1;
        Debug.Log($"[Sequence] �M�~�b�N���������I");
    }

    // ... (Show/HideGimmickCanvas, DisplayMessage �͏ȗ�) ...
    public void DisplayMessage(string message)
    {
        if (canvasController != null)
        {
            canvasController.SetCenterMessage(message);
        }
    }

    public void ShowGimmickCanvas()
    {
        if (canvasController != null) canvasController.gameObject.SetActive(true);
    }

    public void HideGimmickCanvas()
    {
        if (canvasController != null) canvasController.gameObject.SetActive(false);
    }

    // =====================================================
    // GimmickBase�̃��\�b�h
    // =====================================================

    // GimmickBase�ɂ� StartGimmick �͂Ȃ��̂ŁA���ۃN���XGimmickBase�̃��\�b�h�͏ȗ����܂��B
    // StartGimmick���K�v�ȏꍇ�́A�ȉ��̃_�~�[���\�b�h���쐬���Ă��������B

    // public override void StartGimmick(ItemTrigger trigger) { /* �����Ȃ� */ }


    // ������ GimmickTrigger�̖������ւ��郁�\�b�h�͂��̂܂܈ێ� ������
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (currentStage >= 1 && canvasController != null) ShowGimmickCanvas();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            HideGimmickCanvas();
        }
    }

    void Update()
    {
        if (currentStage >= correctSequence.Count + 1) return;
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            StartSequence(); // �C����̃��\�b�h���Ăяo��
        }
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        // ���[�h��̕�������
        inputSequence.Clear();

        // currentStage��1�ȏ�̏ꍇ�A�C���v�b�g�V�[�P���X�𕜌�
        if (currentStage > 0 && currentStage <= correctSequence.Count)
        {
            inputSequence = Enumerable.Repeat(0, currentStage - 1).ToList();
        }
    }
}