using UnityEngine;

public class handleUse: GimmickBase
{
    [Header("必要なアイテム")]
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
            Debug.Log(requiredItem.itemName + " を使って仕掛けを解いた！");
            InventoryManager.Instance.RemoveItemByID(requiredItem.itemID);
            Complete(trigger);
        }
        else
        {
            Debug.Log("このギミックでは使えないアイテムです");
        }
    }

    public override void StartGimmick(ItemTrigger trigger)
    {
        Debug.Log("アイテムスロットから使用してください");
    }
}