using UnityEngine;

/// <summary>
/// 鍵アイテムの効果
/// プレイヤーが近くのドアの前に立っているときのみ使用できる。
/// 向き・鍵IDが一致していれば開錠。それ以外では使用不可。
/// </summary>
[CreateAssetMenu(menuName = "Game/Effects/KeyEffect")]
public class KeyEffect : ItemEffect
{
    public override bool CanExecute(ItemData item)
    {
        //  ドアが近くにあり、向きと鍵IDが一致している場合のみ true
        var door = DoorController.PlayerNearbyDoor;
        if (door == null) return false;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        var move = player.GetComponent<GridMovement>();
        if (move == null) return false;

        // 向きが一致していなければ使えない
        if (move.GetDirection() != door.RequiredDirection)
            return false;

        // 鍵が必要ないドアは対象外
        string requiredKey = door.GetRequiredKeyID();
        if (string.IsNullOrEmpty(requiredKey)) return false;

        // 鍵IDが一致しているときのみ使用可能
        return requiredKey == item.itemID && door.currentStage == 0;
    }

    public override void Execute(ItemData item)
    {
        var door = DoorController.PlayerNearbyDoor;
        if (door == null)
        {
            Debug.Log("[KeyEffect] ドアの前にいないため使用できません。");
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var move = player.GetComponent<GridMovement>();
        if (move == null) return;

        // 向きが一致していなければ無効
        if (move.GetDirection() != door.RequiredDirection)
        {
            Debug.Log("[KeyEffect] 向きが違うため鍵を使えません。");
            return;
        }

        // 必要な鍵が一致しているか確認
        string requiredKey = door.GetRequiredKeyID();
        if (requiredKey != item.itemID)
        {
            Debug.Log("[KeyEffect] この鍵はこのドアに合いません。");
            return;
        }

        // 解錠済みならスキップ
        if (door.currentStage == 1)
        {
            Debug.Log("[KeyEffect] すでに開いています。");
            return;
        }

        //  解錠実行
        bool success = door.TryUseKey(item.itemID);
        if (success)
        {
            Debug.Log($"[KeyEffect] {item.itemName} を使って {door.name} を開けた！");

            if (item.isConsumable)
                InventoryManager.Instance.RemoveItemByID(item.itemID);
        }
    }
}