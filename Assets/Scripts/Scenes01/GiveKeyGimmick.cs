using UnityEngine;

public class GiveKeyGimmick : GimmickBase
{
    [Header("必要なアイテム")]
    public ItemData requiredItem;

    public override bool NeedsItem => true;

    // このギミックがアイテムを受け付けるか判定
    public override bool CanUseItem(ItemData item)
    {
        return item == requiredItem;
    }

    // アイテム使用処理
    public override void UseItem(ItemData usedItem, ItemTrigger trigger)
    {
        if (usedItem == requiredItem)
        {
            Debug.Log(requiredItem.itemName + " を使って扉を開けた！");
            InventoryManager.Instance.RemoveItemByID(requiredItem.itemID);

            // ギミック完了
            Complete(trigger);
        }
        else
        {
            Debug.Log("このギミックでは使えないアイテムです");
        }
    }

    // ギミック開始（Enter直押しは無効）
    public override void StartGimmick(ItemTrigger trigger)
    {
        Debug.Log("アイテムスロットから使用してください");
    }
}