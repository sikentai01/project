using UnityEngine;

[System.Serializable]
public class GiveKeyGimmick : GimmickBase
{
    [Header("使用するアイテムID（KeyBandのID）")]
    public string requiredItemID;

    [Header("入手するアイテムデータ")]
    public ItemData itemToGive;

    // ギミックの開始時に実行される
    public override void StartGimmick(ItemTrigger trigger)
    {
        // 必要なアイテムを持っているかチェック
        if (InventoryManager.Instance.HasItem(requiredItemID))
        {
            // アイテムを消費して入手
            InventoryManager.Instance.RemoveItemByID(requiredItemID);
            InventoryManager.Instance.AddItem(itemToGive);
            Debug.Log(itemToGive.itemName + " を入手しました！");

            // ギミックが完了したことをトリガーに伝える
            trigger.CompleteCurrentGimmick();
        }
        else
        {
            Debug.Log("アイテムが足りません。");
        }
    }
}
