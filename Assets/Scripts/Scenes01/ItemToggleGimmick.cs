using UnityEngine;
using System.Collections;

/// <summary>
/// �A�C�e���g�p��Enter�L�[�ŏ�Ԃ��g�O�����A�A�C�e���̏���E�������s���M�~�b�N
/// Stage 0: �ҋ@��� (�I�u�W�F�N�g��\���A�A�C�e�����茳�ɂ�����)
/// Stage 1: �ݒu��� (�I�u�W�F�N�g�\���A�A�C�e��������ꂽ���)
/// </summary>
public class ItemToggleGimmick : GimmickBase
{
    [Header("���̃M�~�b�N�Ńg�O������A�C�e���f�[�^")]
    public ItemData toggleItemData;

    [Header("�\��/��\����؂�ւ���^�[�Q�b�g")]
    public GameObject targetObject;

    [Header("�Ή�����GimmickTrigger��ID")] // Enter�L�[���͈͂̔͂���肷�邽�߂Ɏg�p
    public string triggerIDToMonitor;

    private GimmickTrigger associatedTrigger; // �Ď��Ώۂ̃g���K�[
    private bool isInitialized = false;

    private void Awake()
    {
        // �v���C���[�̈ʒu�m�F�̂��߂� Awake �Ńg���K�[��T��
        if (!string.IsNullOrEmpty(triggerIDToMonitor))
        {
            var triggers = FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
            foreach (var trigger in triggers)
            {
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
        UpdateVisualState();
    }

    private void Update()
    {
        // �v���C���[�̐ڋߔ���� associatedTrigger �o�R�ōs��
        bool isPlayerInZone = associatedTrigger != null && associatedTrigger.IsPlayerNear;

        // �ݒu��� (Stage 1) �Ńv���C���[���͈͓��ɂ���ꍇ�� Enter �L�[���͂����o
        if (currentStage == 1 && isPlayerInZone && Input.GetKeyDown(KeyCode.Return))
        {
            // �ݒu��Ԃ���ҋ@��Ԃփg�O������
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
        if (currentStage != 0) return false;

        if (targetObject == null || toggleItemData == null)
        {
            Debug.LogWarning("[ItemToggleGimmick] �^�[�Q�b�g�܂��̓A�C�e�������ݒ�ł��B");
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
        if (currentStage != 1) return;

        if (targetObject == null || toggleItemData == null)
        {
            Debug.LogWarning("[ItemToggleGimmick] �^�[�Q�b�g�܂��̓A�C�e�������ݒ�ł��B");
            return;
        }

        // �M�~�b�N�����ƃA�C�e������
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
        if (targetObject == null) return;

        // Stage 1 �Ȃ�\���AStage 0 �Ȃ��\��
        targetObject.SetActive(currentStage == 1);
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);
        UpdateVisualState();
    }
}