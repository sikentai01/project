using UnityEngine;

public class DummyGimmick : GimmickBase
{
    public ItemData requiredItem;  // 必要なアイテム（例：水）

    public override void StartGimmick(ItemTrigger trigger)
    {
        // プレイヤーが必要なアイテムを持っているかチェック
        if (InventoryManager.Instance.items.Contains(requiredItem))
        {
            Debug.Log(requiredItem.itemName + " を使って仕掛けを解いた！");
            InventoryManager.Instance.RemoveItemByID(requiredItem.itemID);

            // ギミック完了 → ItemTrigger の CompleteCurrentGimmick が呼ばれる
            Complete(trigger);
        }
        else
        {
            Debug.Log("必要なアイテムを持っていない");
        }
    }
}