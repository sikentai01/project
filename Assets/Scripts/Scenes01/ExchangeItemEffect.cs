using UnityEngine;

[CreateAssetMenu(menuName = "Gimmick/Exchange Item Effect", fileName = "ExchangeItemEffect")]
public class ExchangeItemEffect : GimmickEffectBase
{
    [Header("���삷��M�~�b�NID (�C�ӁF�߂��ɕ����̃M�~�b�N������ꍇ)")]
    public string targetGimmickID;

    /// <summary>
    /// �A�C�e�����g�p�\�����肷��B�߂��ɑΏۂ�GimmickTrigger������ꍇ�̂� true�B
    /// </summary>
    public override bool CanExecute(ItemData item)
    {
        // �V�[�����̂��ׂĂ�GimmickTrigger��T��
        var triggers = Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            // 1. �v���C���[���g���K�[�͈͓̔��ɂ��邩�H
            if (!trigger.IsPlayerNear) continue;

            // 2. ���̃g���K�[�� ItemExchangeGimmick ��ێ����Ă��邩�H
            var gimmick = trigger.GetGimmick<ItemExchangeGimmick>();
            if (gimmick != null)
            {
                // 3. �M�~�b�NID�̎w�肪����΁A�������v���Ă��邩�H
                if (string.IsNullOrEmpty(targetGimmickID) || gimmick.gimmickID == targetGimmickID)
                {
                    // ���ׂĂ̏����𖞂����΁A�A�C�e���g�p������
                    return true;
                }
            }
        }

        // �߂��ɑΏۂ̃M�~�b�N��������Ȃ��ꍇ�͎g�p�s��
        Debug.LogWarning($"[ExchangeItemEffect] GimmickTrigger�̃R���C�_�[�͈͊O���A�ΏۃM�~�b�N��������܂���B");
        return false;
    }

    public override void Execute(ItemData usedItem)
    {
        // CanExecute() �� true ��Ԃ��Ă��邽�߁AExecute�����s����� = �߂��ɑΏۃM�~�b�N������͂�

        // GimmickEffectBase�̋��ʃ��\�b�h�ŃM�~�b�N�����s
        bool success = TryInvokeNearbyGimmick<ItemExchangeGimmick>(gimmick =>
        {
            // �M�~�b�NID�̃`�F�b�N
            if (!string.IsNullOrEmpty(targetGimmickID) && gimmick.gimmickID != targetGimmickID)
                return;

            // �A�C�e���������������s
            if (gimmick.ExecuteExchange())
            {
                // ���������ꍇ�̂݃A�C�e�����폜
                InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            }
        });

        if (!success)
        {
            // CanExecute()��true�ł��A���炩�̗��R��Execute�����s�����ꍇ�́A�A�C�e���͍폜���ꂸ�Ɏc��
            Debug.LogWarning($"[ExchangeItemEffect] �M�~�b�N�̎��s���Ɏ��s���܂����B�A�C�e���͍폜����܂���ł����B");
        }
    }
}