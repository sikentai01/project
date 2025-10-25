using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// �A�C�e���g�p��Enter�L�[�ŏ�Ԃ��g�O�����A�A�C�e���̏���E�������s���M�~�b�N
/// Stage 0: �ҋ@��� (�I�u�W�F�N�g��OFF)
/// Stage 1: �ݒu��� (�I�u�W�F�N�g��ON)
/// </summary>
public class ItemToggleGimmick : GimmickBase
{
    [Header("���̃M�~�b�N�Ńg�O������A�C�e���f�[�^")]
    public ItemData toggleItemData;

    [Header("�\��/��\����؂�ւ���I�u�W�F�N�g�Q")]
    public GameObject[] targetObjects; // Stage 1��ON�ɂȂ�O���[�v

    [Header("�\��/��\�����t�ɐ؂�ւ���I�u�W�F�N�g�Q (Stage 1��OFF)")]
    public GameObject[] offObjects; // Stage 1��OFF�ɂȂ�O���[�v

    [Header("�Ή�����GimmickTrigger��ID")] // Enter�L�[���͈͂̔͂���肷�邽�߂Ɏg�p
    public string triggerIDToMonitor;

    // �� ���Z�b�g�@�\�̃t�B�[���h�͍���͖������āA���W�b�N�̂ݏC�����܂�

    private GimmickTrigger associatedTrigger; // �Ď��Ώۂ̃g���K�[

    // Enter�L�[���̗͂L���͈̓`�F�b�N�Ɏg��
    private bool IsPlayerNear => associatedTrigger != null && associatedTrigger.IsPlayerNear;
    // private bool isLoaded = false; // �s�v�ɂȂ������ߍ폜

    private void Awake()
    {
        // �v���C���[�̈ʒu�m�F�̂��߂� Awake �Ńg���K�[��T��
        if (!string.IsNullOrEmpty(triggerIDToMonitor))
        {
            var triggers = FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
            foreach (var trigger in triggers)
            {
                // GimmickTrigger��GimmickID�������Ă��܂�
                if (trigger.gimmickID == triggerIDToMonitor)
                {
                    associatedTrigger = trigger;
                    break;
                }
            }
        }

        if (associatedTrigger == null && !string.IsNullOrEmpty(triggerIDToMonitor))
        {
            Debug.LogWarning($"[ItemToggleGimmick] ID:{triggerIDToMonitor} ��GimmickTrigger��������܂���BEnter�L�[���͂������ɂȂ�܂��B");
        }
    }

    private void Start()
    {
        // currentStage�Ɋ�Â��ď�����Ԃ�ݒ�iLoadProgress�ŏ㏑�������\������j
        UpdateVisualState();

        // LoadProgress�̌��InventoryManager�`�F�b�N���K�v�Ȃ�A������StartCoroutine���Ă�
    }

    private void Update()
    {
        // �ݒu��� (Stage 1) �Ńv���C���[���͈͓��ɂ���ꍇ�� Enter �L�[���͂����o
        if (currentStage == 1 && IsPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // �ݒu��Ԃ���ҋ@��Ԃփg�O������ (�A�C�e����������\��)
            TryRestoreItemAndHide();
        }
    }

    // =====================================================
    // �O������̑��� (ItemEffect.Execute����Ă΂��)
    // =====================================================

    /// <summary>
    /// �A�C�e�����g�p���ăM�~�b�N��ݒu���� (Stage 0 �� 1)
    /// </summary>
    public bool TryPlaceAndConsumeItem()
    {
        if (currentStage != 0) return false; // ���ɐݒu�ς݂̏ꍇ�͎��s

        if (targetObjects.Length == 0 || toggleItemData == null)
        {
            Debug.LogWarning("[ItemToggleGimmick] �^�[�Q�b�g�I�u�W�F�N�g�܂��̓A�C�e�������ݒ�ł��B");
            return false;
        }

        // �ݒu����
        this.currentStage = 1;
        UpdateVisualState();
        Debug.Log($"[ItemToggleGimmick] {toggleItemData.itemName} ���g�p���Đݒu (Stage 1)�B");
        return true;
    }

    // =====================================================
    // ��������̑��� (Update/Enter�L�[����Ă΂��)
    // =====================================================

    /// <summary>
    /// Enter�L�[�������ăM�~�b�N���������A�A�C�e���𕜌����� (Stage 1 �� 0)
    /// </summary>
    private void TryRestoreItemAndHide()
    {
        if (currentStage != 1) return; // �ݒu�ς݂łȂ���Ύ��s

        if (toggleItemData == null)
        {
            Debug.LogWarning("[ItemToggleGimmick] �g�O���A�C�e�������ݒ�ł��B�A�C�e�������ł��܂���B");
            return;
        }

        // �M�~�b�N�����ƃA�C�e������
        // �� �����ł�AddItem�� InventoryManager�� HasItem�`�F�b�N�ƘA�����A������������ꍇ��
        //    AddItem������ TryRestoreItemAndHide �̌Ăяo�����Ő��䂷��K�v������܂��B
        InventoryManager.Instance.AddItem(toggleItemData);
        Debug.Log($"[ItemToggleGimmick] �M�~�b�N�����B{toggleItemData.itemName} �𕜌����܂��� (Stage 0)�B");

        this.currentStage = 0;
        UpdateVisualState();
    }

    // =====================================================
    // ��ԍX�V�ƃZ�[�u�E���[�h
    // =====================================================

    private void UpdateVisualState()
    {
        bool shouldBeActive = (currentStage == 1); // Stage 1�Ȃ�ON�A0�Ȃ�OFF

        // targetObjects: Stage 1��ON�ɂȂ�O���[�v
        foreach (var obj in targetObjects)
        {
            if (obj != null)
                obj.SetActive(shouldBeActive);
        }

        // offObjects: Stage 1��OFF�ɂȂ�O���[�v
        foreach (var obj in offObjects)
        {
            if (obj != null)
                obj.SetActive(!shouldBeActive);
        }
    }

    public override void LoadProgress(int stage)
    {
        // ������ �C���_: �e�N���X (GimmickBase) �̎������Ăяo���A�Z�[�u�f�[�^�ʂ�̐i�s�i�K�𕜌����� ������
        base.LoadProgress(stage);

        // ���[�h���ꂽ�i�s�i�K�Ɋ�Â��Č����ڂ��X�V
        UpdateVisualState();

        Debug.Log($"[ItemToggleGimmick: {gimmickID}] ���[�h����: �Z�[�u�l {stage} �𕜌����܂����B");
    }
    // �ǂ����̃X�N���v�g�� (��)


}