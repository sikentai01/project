using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effects/HandleEffect")]
public class HandleEffect : ItemEffect
{
    public override bool CanExecute(ItemData item)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        var movement = player.GetComponent<GridMovement>();
        if (movement == null) return false;

        // 上を向いている時だけ実行可能
        return movement.GetDirection() == 3;
    }

    public override void Execute(ItemData item)
    {
        Debug.Log(item.itemName + " を使って水をまいた！");

        if (item.isConsumable)
        {
            InventoryManager.Instance.RemoveItemByID(item.name);
        }
    }
}