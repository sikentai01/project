using UnityEngine;

public class handleUse: GimmickBase
{
    [Header("�K�v�ȃA�C�e��")]
    public ItemData requiredItem;

    public override bool NeedsItem => true;

    public override bool CanUseItem(ItemData item)
    {
        return item == requiredItem;
    }

    public override void UseItem(ItemData usedItem, ItemTrigger trigger)
    {
        if (usedItem == requiredItem)
        {
            Debug.Log(requiredItem.itemName + " ���g���Ďd�|�����������I");
            InventoryManager.Instance.RemoveItemByID(requiredItem.itemID);
            Complete(trigger);
        }
        else
        {
            Debug.Log("���̃M�~�b�N�ł͎g���Ȃ��A�C�e���ł�");
        }
    }

    public override void StartGimmick(ItemTrigger trigger)
    {
        Debug.Log("�A�C�e���X���b�g����g�p���Ă�������");
    }
}