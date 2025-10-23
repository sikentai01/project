using System.Collections.Generic;
using UnityEngine;

// �M�~�b�N�̊�{�N���X���p��
public class ItemExchangeGimmick : GimmickBase
{
    [Header("�����ɐ���������ɐ��������A�C�e��")]
    public ItemData rewardItemData;

    [Header("��������A�C�e���̌����ڃI�u�W�F�N�g (������\��)")]
    public GameObject rewardObject;

    // �� �ǉ��t�B�[���h�F�J��Ԃ��\��
    [Header("�A�C�e�����������x�ł��\�ɂ��邩")]
    public bool isRepeatable = false;

    // �ȉ��̓���ȃT�C�h�G�t�F�N�g�� currentStage == 0 �̎��̂ݎ��s
    [Header("�����������ɖ���������GimmickTrigger�̃��X�g (����̂�)")]
    public List<GimmickTrigger> triggersToDisable = new List<GimmickTrigger>();

    [Header("����������ɕ�����h�A (����̂�)")]
    public DoorController doorToLock;

    [Header("����������ɃA�N�e�B�u�ɂ���GameOver�R���g���[���[ (����̂�)")]
    public GameObject gameOverControllerObject;


    /// <summary>
    /// �A�C�e���������������s����
    /// </summary>
    /// <returns>�����ɐ��������� true</returns>
    public bool ExecuteExchange()
    {
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ExecuteExchange���Ă΂�܂����B");

        // ���Ɋ����ς݁icurrentStage��1�ȏ�j���A�J��Ԃ��s��(isRepeatable=false)�Ȃ玸�s
        if (!isRepeatable && this.currentStage >= 1)
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

        // 1. �h�A��� (currentStage��0�ɐݒ�) - ������s����
        if (doorToLock != null)
        {
            // DoorController�� LoadProgress(0) ���Ăяo���A��Ԃ���iStage 0�j�ɐݒ�
            doorToLock.LoadProgress(0);
            Debug.Log($"[ItemExchangeGimmick] {doorToLock.gameObject.name} ������܂��� (Stage 0)�B");
        }

        // 2. ��V�A�C�e����ǉ� (�����������s)
        InventoryManager.Instance.AddItem(rewardItemData);
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardItemData.name} ({rewardItemData.itemID}) ���C���x���g���ɒǉ����܂����B");


        // 3. ����̂ݎ��s�������ȃT�C�h�G�t�F�N�g�̏���
        if (this.currentStage == 0) // currentStage �� 0 (�������) �̏ꍇ�̂ݎ��s
        {
            // A. �g���K�[�𖳌���
            DisableTriggers();

            // B. �Q�[���I�[�o�[�����̎��s
            if (gameOverControllerObject != null)
            {
                gameOverControllerObject.SetActive(true);
                Debug.Log("[ItemExchangeGimmick] ������s �� �Q�[���I�[�o�[�N��");
            }

            // C. �i�s�x���X�V�i�����Ƃ���j�F����ɂ��Z�[�u����A����ȍ~�̂��̃u���b�N�̓X�L�b�v�����
            this.currentStage = 1;
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ���񊮗�(Stage 1)�ɐݒ肵�܂����B");
        }


        // 4. ��V�A�C�e���̌����ڂ�\���i�ݒ肳��Ă���ꍇ�j
        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardObject.name} ��\�����܂����B");
        }

        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ��������: {rewardItemData.itemName} �̓��菈�������B");

        return true;
    }

    /// <summary>
    /// �ݒ肳�ꂽ�g���K�[�𖳌�������
    /// </summary>
    private void DisableTriggers()
    {
        // ... (DisableTriggers �̒��g�͕ύX�Ȃ�) ...
        foreach (var trigger in triggersToDisable)
        {
            if (trigger != null)
            {
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
        if (this.currentStage >= 1 && !isRepeatable) // �J��Ԃ��s�̃M�~�b�N�̂݃��[�h����ԕ���
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