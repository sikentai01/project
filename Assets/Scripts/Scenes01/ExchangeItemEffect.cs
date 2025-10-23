using UnityEngine;

[CreateAssetMenu(menuName = "Gimmick/Exchange Item Effect", fileName = "ExchangeItemEffect")]
public class ExchangeItemEffect : GimmickEffectBase
{
    [Header("���삷��M�~�b�NID (�C��)")]
    public string targetGimmickID;

    [Header("�K�v�Ȍ��� (0=��, 1=��, 2=�E, 3=��, -1=�����Ȃ�)")]
    public int requiredDirection = -1; // �� �V�K�ǉ�

    /// <summary>
    /// �A�C�e�����g�p�\�����肷��B�߂��ɑΏۂ�GimmickTrigger������ꍇ�A���������������ꍇ�̂� true�B
    /// </summary>
    public override bool CanExecute(ItemData item)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        // �� 1. �����̃`�F�b�N
        if (requiredDirection != -1)
        {
            var movement = player.GetComponent<GridMovement>();
            if (movement == null) return false;

            if (movement.GetDirection() != requiredDirection)
            {
                Debug.LogWarning($"[ExchangeItemEffect] ����������������܂���B (�K�v: {requiredDirection}, ����: {movement.GetDirection()})");
                return false;
            }
        }

        // �� 2. �R���C�_�[�͈͂̃`�F�b�N (�������W�b�N)
        var triggers = Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            if (!trigger.IsPlayerNear) continue;

            var gimmick = trigger.GetGimmick<ItemExchangeGimmick>();
            if (gimmick != null)
            {
                if (string.IsNullOrEmpty(targetGimmickID) || gimmick.gimmickID == targetGimmickID)
                {
                    // �����̃`�F�b�N�ƃR���C�_�[�̃`�F�b�N�̗�����ʉ�
                    return true;
                }
            }
        }

        Debug.LogWarning($"[ExchangeItemEffect] GimmickTrigger�̃R���C�_�[�͈͊O���A�ΏۃM�~�b�N��������܂���B");
        return false;
    }

    public override void Execute(ItemData usedItem)
    {
        // ... (Execute �̒��g�͕ύX�Ȃ�) ...

        bool success = TryInvokeNearbyGimmick<ItemExchangeGimmick>(gimmick =>
        {
            if (!string.IsNullOrEmpty(targetGimmickID) && gimmick.gimmickID != targetGimmickID)
                return;

            if (gimmick.ExecuteExchange())
            {
                InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            }
        });

        if (!success)
        {
            Debug.LogWarning($"[ExchangeItemEffect] �M�~�b�N�̎��s���Ɏ��s���܂����B�A�C�e���͍폜����܂���ł����B");
        }
    }
}