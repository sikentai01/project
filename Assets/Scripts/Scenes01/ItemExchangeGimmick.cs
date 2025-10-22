using UnityEngine;

// GimmickBase���p�����A�A�C�e���̌����E�������Ǘ�����
public class ItemExchangeGimmick : GimmickBase
{
    [Header("�����ɐ���������ɐ��������A�C�e��")]
    public ItemData rewardItemData;

    [Header("��������A�C�e���̌����ڃI�u�W�F�N�g (������\��)")]
    public GameObject rewardObject;

    /// <summary>
    /// �A�C�e���������������s����
    /// </summary>
    /// <returns>�����ɐ��������� true</returns>
    public bool ExecuteExchange()
    {
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ExecuteExchange���Ă΂�܂����B"); // �� �ǉ����O

        // ���Ɍ����ς݁icurrentStage��1�ȏ�j�Ȃ玸�s
        if (this.currentStage >= 1)
        {
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] �M�~�b�N�͊��Ɋ������Ă��܂��B���s���X�L�b�v���܂��B"); // �� �ǉ����O
            return false;
        }

        if (rewardItemData == null)
        {
            Debug.LogWarning($"[ItemExchangeGimmick: {gimmickID}] ��V�A�C�e�����ݒ肳��Ă��܂���B���s�B"); // �� �ǉ����O
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError($"[ItemExchangeGimmick: {gimmickID}] InventoryManager.Instance��NULL�ł��B�A�C�e����ǉ��ł��܂���B"); // �� �ǉ����O
            return false;
        }

        // �C���x���g���ɕ�V�A�C�e����ǉ�
        InventoryManager.Instance.AddItem(rewardItemData);
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardItemData.name} ({rewardItemData.itemID}) ���C���x���g���ɒǉ����܂����B"); // �� �ǉ����O

        // �i�s�x���X�V�i�����Ƃ���j
        this.currentStage = 1;

        // ��V�A�C�e���̌����ڂ�\���i�ݒ肳��Ă���ꍇ�j
        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardObject.name} ��\�����܂����B");
        }

        // �M�~�b�N�������O
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ��������: {rewardItemData.itemName} ����菈�������B");

        return true;
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        // ���[�h���ɃM�~�b�N���������Ă���ꍇ�A��V�̌����ڂ𕜌�
        if (this.currentStage >= 1 && rewardObject != null)
        {
            rewardObject.SetActive(true);
        }
    }
}