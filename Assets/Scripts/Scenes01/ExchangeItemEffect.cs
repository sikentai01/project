using UnityEngine;

[CreateAssetMenu(menuName = "Gimmick/Exchange Item Effect", fileName = "ExchangeItemEffect")]
public class ExchangeItemEffect : GimmickEffectBase
{
    [Header("���삷��M�~�b�NID (�C�ӁF�߂��ɕ����̃M�~�b�N������ꍇ)")]
    public string targetGimmickID;

    /// <summary>
    /// �A�C�e���X���b�g����̎g�p�������邽�߁A��� true ��Ԃ�
    /// </summary>
    public override bool CanExecute(ItemData item)
    {
        // ��Ɏg�p�\�Ƃ���B�R���C�_�[�`�F�b�N�� Execute ���ōs���B
        return true;
    }

    public override void Execute(ItemData usedItem)
    {
        // TryInvokeNearbyGimmick�ŁA�߂��� ItemExchangeGimmick ��T���Ď��s����
        bool success = TryInvokeNearbyGimmick<ItemExchangeGimmick>(gimmick =>
        {
            // �M�~�b�NID���ݒ肳��Ă���΃`�F�b�N
            if (!string.IsNullOrEmpty(targetGimmickID) && gimmick.gimmickID != targetGimmickID)
                return;

            // �A�C�e���������������s
            if (gimmick.ExecuteExchange())
            {
                // �����ɐ���������A�g�p�A�C�e�����C���x���g������ID���g���č폜
                InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            }
        });

        if (!success)
        {
            // �M�~�b�N��������Ȃ������A�܂��� IsPlayerNear �͈̔͊O�������ꍇ�̃��b�Z�[�W
            Debug.LogWarning($"[ExchangeItemEffect] �߂��ɑΏۂ̃M�~�b�N��������܂���B�A�C�e���͍폜����܂���ł����B");
        }
    }
}