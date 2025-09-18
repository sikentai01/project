using UnityEngine;

public class DummyGimmick : GimmickBase
{
    public ItemData requiredItem;  // �K�v�ȃA�C�e���i��F���j

    public override void StartGimmick(ItemTrigger trigger)
    {
        // �v���C���[���K�v�ȃA�C�e���������Ă��邩�`�F�b�N
        if (InventoryManager.Instance.items.Contains(requiredItem))
        {
            Debug.Log(requiredItem.itemName + " ���g���Ďd�|�����������I");
            InventoryManager.Instance.RemoveItemByID(requiredItem.itemID);

            // �M�~�b�N���� �� ItemTrigger �� CompleteCurrentGimmick ���Ă΂��
            Complete(trigger);
        }
        else
        {
            Debug.Log("�K�v�ȃA�C�e���������Ă��Ȃ�");
        }
    }
}