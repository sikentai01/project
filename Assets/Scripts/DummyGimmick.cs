using UnityEngine;

public class DummyGimmick : GimmickBase
{
    public ItemData requiredItem;  // 必要なアイテム（例：水）

    // ItemSlotから呼ばれる用のメソッド
    public void UseItemForGimmick(ItemData usedItem, ItemTrigger trigger)
    {
        if (usedItem == requiredItem)
        {
            Debug.Log(requiredItem.itemName + " を使って仕掛けを解いた！");
            InventoryManager.Instance.RemoveItemByID(requiredItem.itemID);

            // ギミック完了
            Complete(trigger);
        }
        else
        {
            Debug.Log("このギミックでは使えないアイテムです");
        }
    }

    // 直接Enterで呼ばれるのはもう無し
    public override void StartGimmick(ItemTrigger trigger)
    {
        Debug.Log("アイテムスロットから使用してください");
    }
}