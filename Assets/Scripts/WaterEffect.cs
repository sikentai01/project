using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effects/WaterEffect")]
public class WaterEffect : ItemEffect
{
    public override void Execute(ItemData item)
    {
        Debug.Log(item.itemName + " を使って水をまいた！");
        // ここに効果を実装
        if (item.isConsumable)
        {
            InventoryManager.Instance.RemoveItemByID(item.name);
            // item.name じゃなくて、別に itemID プロパティを追加してもいい
        }
    }
}