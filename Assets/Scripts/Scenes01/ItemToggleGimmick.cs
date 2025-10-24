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

    // ������ �ǉ��t�B�[���h ������
    [Header("��������Stage 0�Ƀ��Z�b�g����A�C�e��ID")]
    public string resetIfHasItemID = "";
    // ������ �����܂� ������

    private GimmickTrigger associatedTrigger; // �Ď��Ώۂ̃g���K�[

    // Enter�L�[���̗͂L���͈̓`�F�b�N�Ɏg��
    private bool IsPlayerNear => associatedTrigger != null && associatedTrigger.IsPlayerNear;
    private bool isLoaded = false;

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
        // currentStage�Ɋ�Â��ď�����Ԃ�ݒ�iLoadProgress�ŏ㏑�������\������j
        UpdateVisualState();

        // ������ ���[�h��̏�ԃ`�F�b�N�� StartCoroutine �Ŏ��s ������
        if (gimmickID != "") // ID���ݒ肳��Ă���M�~�b�N�̂݃`�F�b�N
        {
            StartCoroutine(CheckInventoryAndApplyLoadState());
        }
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
        // LoadProgress�ł́A�Z�[�u�f�[�^�̒l (stage) �����̂܂ܓK�p����̂�
        // ���Z�b�g������ CheckInventoryAndApplyLoadState() �ɔC����
        this.currentStage = stage;
        UpdateVisualState();
        isLoaded = true;
    }

    // ������ ItemTrigger�Ɠ��l�̒x���`�F�b�N���W�b�N�i���[�h��̖��������j ������
    private IEnumerator CheckInventoryAndApplyLoadState()
    {
        // InventoryManager �������������܂őҋ@
        while (InventoryManager.Instance == null)
        {
            yield return null;
        }

        // InventoryManager���Z�[�u�f�[�^���[�h����������̂�҂��߂̗P�\
        yield return new WaitForSeconds(0.1f);

        // ���� LoadProgress �ŃZ�[�u�f�[�^���K�p����Ă���͂��Ȃ̂ŁA���̏�Ԃ��`�F�b�N

        string itemID = resetIfHasItemID;
        if (!string.IsNullOrEmpty(itemID) && InventoryManager.Instance != null)
        {
            bool isItemInInventory = InventoryManager.Instance.HasItem(itemID);

            if (isItemInInventory)
            {
                // �A�C�e���������Ă���ꍇ�́A�Z�[�u�f�[�^�̒l�����ł��� Stage 0 �ɋ������Z�b�g
                this.currentStage = 0;
                UpdateVisualState();
                Debug.Log($"[ItemToggleGimmick:{gimmickID}] **�x���`�F�b�N����**: �A�C�e�������ɂ�� Stage 0 �Ƀ��Z�b�g���܂����B");
            }
        }
    }
}