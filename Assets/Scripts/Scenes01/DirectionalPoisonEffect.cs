using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effects/Directional Poison", fileName = "DirectionalPoisonEffect")]
public class DirectionalPoisonEffect : ItemEffect
{
    [Header("発動に必要な向き (0=下, 1=左, 2=右, 3=上, -1=制限なし)")]
    public int requiredDirection = -1;

    [Header("対象となるGimmickTriggerのID (空欄で近くのどのGimmickTriggerでも可)")]
    public string targetTriggerID;

    [Header("使用時にインベントリから削除するか")]
    public bool isConsumable = true;

    /// <summary>
    /// アイテム使用の条件：特定のコライダー内にいて、特定の方向を向いているかチェックする
    /// </summary>
    public override bool CanExecute(ItemData item)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        // 1. 向きのチェック
        if (requiredDirection != -1)
        {
            var movement = player.GetComponent<GridMovement>();
            if (movement == null) return false;

            if (movement.GetDirection() != requiredDirection)
            {
                // 向きが違うため使用不可
                return false;
            }
        }

        // 2. コライダー範囲のチェック
        var triggers = Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        bool isInValidArea = false;

        foreach (var trigger in triggers)
        {
            if (!trigger.IsPlayerNear) continue; // プレイヤーが範囲外

            // ターゲットIDが設定されている場合はIDをチェック
            if (!string.IsNullOrEmpty(targetTriggerID) && trigger.gimmickID != targetTriggerID)
            {
                continue; // IDが一致しないため無視
            }

            // プレイヤーが範囲内にいて、IDも一致した（またはIDが設定されていない）
            isInValidArea = true;
            break;
        }

        if (!isInValidArea)
        {
            Debug.LogWarning("[DirectionalPoisonEffect] プレイヤーは対象のトリガー範囲外です。");
            return false;
        }

        // 向きチェックとコライダーチェックの両方をクリア
        return true;
    }

    /// <summary>
    /// 実行：ゲームオーバーを起動し、アイテムを削除する
    /// </summary>
    public override void Execute(ItemData usedItem)
    {
        // Executeが呼ばれた時点では、CanExecute()がtrueを返していることが保証されている

        // 1. アイテムをインベントリから削除
        if (isConsumable && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            Debug.Log($"[DirectionalPoisonEffect] {usedItem.itemName} を使用し、削除しました。");
        }

        // 2. ゲームオーバー処理の実行 (BootLoaderによるシーン切り替え)
        if (BootLoader.Instance != null)
        {
            BootLoader.Instance.SwitchSceneInstant("GameOver");
            Debug.Log("[DirectionalPoisonEffect] 毒瓶を使用 → GameOverシーンへ即時切り替え");
        }
        else
        {
            Debug.LogError("[DirectionalPoisonEffect] BootLoader.Instanceが見つかりません。GameOver処理をスキップしました。");
        }
    }
}