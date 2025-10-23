using System.Collections.Generic;
using UnityEngine;

// GimmickBase���p�����A�A�C�e���̌����E�����ƃg���K�[�̖������A�h�A�����Ǘ�����
public class ItemExchangeGimmick : GimmickBase
{
    [Header("�����ɐ���������ɐ��������A�C�e��")]
    public ItemData rewardItemData;

    [Header("��������A�C�e���̌����ڃI�u�W�F�N�g (������\��)")]
    public GameObject rewardObject;

    [Header("�����������ɖ���������GimmickTrigger�̃��X�g")]
    public List<GimmickTrigger> triggersToDisable = new List<GimmickTrigger>();

    [Header("����������ɕ�����h�A")]
    public DoorController doorToLock;

    /// <summary>
    /// �A�C�e���������������s����
    /// </summary>
    /// <returns>�����ɐ��������� true</returns>
    public bool ExecuteExchange()
    {
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ExecuteExchange���Ă΂�܂����B");

        // ���Ɍ����ς݁icurrentStage��1�ȏ�j�Ȃ玸�s
        if (this.currentStage >= 1)
        {
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] �M�~�b�N�͊��Ɋ������Ă��܂��B���s���X�L�b�v���܂��B");
            return false;
        }

        if (rewardItemData == null)
        {
            Debug.LogWarning($"[ItemExchangeGimmick: {gimmickID}] ��V�A�C�e�����ݒ肳��Ă��܂���B���s�B");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError($"[ItemExchangeGimmick: {gimmickID}] InventoryManager��NULL�ł��B�A�C�e����ǉ��ł��܂���B");
            return false;
        }

        // --- �������� ---

        // 1. �g���K�[�𖳌���
        DisableTriggers();

        // 2. �h�A��� (currentStage��0�ɐݒ�)
        if (doorToLock != null)
        {
            // DoorController�� LoadProgress(0) ���Ăяo���A��Ԃ���iStage 0�j�ɐݒ�
            doorToLock.LoadProgress(0);
            Debug.Log($"[ItemExchangeGimmick] {doorToLock.gameObject.name} ������܂��� (Stage 0)�B");
        }

        // 3. �C���x���g���ɕ�V�A�C�e����ǉ�
        InventoryManager.Instance.AddItem(rewardItemData);
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardItemData.name} ({rewardItemData.itemID}) ���C���x���g���ɒǉ����܂����B");

        // 4. �i�s�x���X�V�i�����Ƃ���j
        this.currentStage = 1;

        // 5. ��V�A�C�e���̌����ڂ�\���i�ݒ肳��Ă���ꍇ�j
        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardObject.name} ��\�����܂����B");
        }

        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ��������: {rewardItemData.itemName} ����菈�������B");

        return true;
    }

    /// <summary>
    /// �ݒ肳�ꂽ�g���K�[�𖳌�������
    /// </summary>
    private void DisableTriggers()
    {
        foreach (var trigger in triggersToDisable)
        {
            if (trigger != null)
            {
                // GimmickTrigger�R���|�[�l���g�ƃR���C�_�[�𖳌���
                trigger.enabled = false;
                var collider = trigger.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
                Debug.Log($"[ItemExchangeGimmick] �g���K�[�𖳌���: {trigger.gameObject.name}");
            }
        }
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        // ���[�h���ɃM�~�b�N���������Ă���ꍇ�A��V�̌����ڂƃg���K�[�̏�Ԃ𕜌�
        if (this.currentStage >= 1)
        {
            if (rewardObject != null)
            {
                rewardObject.SetActive(true);
            }
            // ������ԂȂ�g���K�[���������𕜌�
            DisableTriggers();
        }
    }
}