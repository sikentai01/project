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


    private void Start()
    {
        // currentStage�Ɋ�Â��ď�����Ԃ�ݒ�
        // LoadProgress���Ă΂��O�ɁAInspector�Őݒ肳�ꂽcurrentStage(�ʏ��0)�ŏ����������
        // LoadProgress���Ă΂ꂽ�炻�̒l�ŏ㏑������AUpdateVisualState()���Ă΂��
        UpdateVisualState();
    }


    /// <summary>
    /// �A�C�e���������������s����
    /// </summary>
    /// <returns>�����ɐ��������� true</returns>
    public bool ExecuteExchange()
    {
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ExecuteExchange���Ă΂�܂����B");

        // 1. ���Ɋ����ς݁icurrentStage��1�ȏ�j���A�J��Ԃ��s��(isRepeatable=false)�Ȃ玸�s
        if (currentStage >= 1 && !isRepeatable)
        {
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ���Ɋ����ς݂ŌJ��Ԃ��s�̂��߁A���s�B");
            return false;
        }

        // 2. ��V�A�C�e�����C���x���g���ɒǉ�
        if (rewardItemData != null)
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(rewardItemData);
                Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardItemData.itemName} ���C���x���g���ɒǉ����܂����B");
            }
            else
            {
                Debug.LogWarning("[ItemExchangeGimmick] InventoryManager.Instance��null�ł��B");
            }
        }
        else
        {
            Debug.LogWarning("[ItemExchangeGimmick] rewardItemData���ݒ肳��Ă��܂���B");
        }


        // 3. ������s�� (Stage 0) �̂݃T�C�h�G�t�F�N�g�����s
        if (currentStage == 0)
        {
            // A. �g���K�[�̖�����
            DisableTriggers();

            // B. �h�A�̎{��
            if (doorToLock != null)
            {
                doorToLock.currentStage = 0; // �h�A����� (DoorController���Ō����ڂ��X�V����z��)
                Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {doorToLock.gameObject.name} ���{�����܂����B");
            }

            // C. �Q�[���I�[�o�[�R���g���[���[���A�N�e�B�u��
            if (gameOverControllerObject != null)
            {
                gameOverControllerObject.SetActive(true);
                Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {gameOverControllerObject.name} ���A�N�e�B�u�ɂ��܂����B");
            }

            // D. �i�s�x���X�V�i�����Ƃ���j
            this.currentStage = 1;
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ���񊮗�(Stage 1)�ɐݒ肵�܂����B");
        }


        // 4. ��V�A�C�e���̌����ڂ�\���i�ݒ肳��Ă���ꍇ�j
        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardObject.name} ��\�����܂����B");
        }

        // Stage 1 �ɂȂ�����̌J��Ԃ��������� currentStage �̕ύX�͂Ȃ�

        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] �������������B");

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

    /// <summary>
    /// ���݂̐i�s�i�K�Ɋ�Â��I�u�W�F�N�g�̏�Ԃ��X�V
    /// </summary>
    private void UpdateVisualState()
    {
        // ��V�I�u�W�F�N�g�̕\��/��\���� currentStage �Ɋ�Â��Č���
        if (rewardObject != null)
        {
            // Stage 1�ȏ�Ȃ�\��
            rewardObject.SetActive(currentStage >= 1);
        }

        // ���̑��̎��o�I�ȕω����K�v�ł���΂����ɒǉ�
    }

    // =====================================================
    // �Z�[�u�E���[�h
    // =====================================================

    public override void LoadProgress(int stage)
    {
        // ������ �C���_: �e�N���X (GimmickBase) �̎������Ăяo���A�Z�[�u�f�[�^�ʂ�̐i�s�i�K�𕜌����� ������
        base.LoadProgress(stage);

        // ���[�h���ꂽ�i�s�i�K�Ɋ�Â��Č����ڂ��X�V
        UpdateVisualState();

        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ���[�h����: �Z�[�u�l {stage} �𕜌����܂����B");
    }
}