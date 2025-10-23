using UnityEngine;

[CreateAssetMenu(menuName = "Gimmick/Exchange Item Effect", fileName = "ExchangeItemEffect")]
public class ExchangeItemEffect : GimmickEffectBase
{
    [Header("操作するギミックID (任意)")]
    public string targetGimmickID;

    [Header("必要な向き (0=下, 1=左, 2=右, 3=上, -1=制限なし)")]
    public int requiredDirection = -1; // ★ 新規追加

    /// <summary>
    /// アイテムが使用可能か判定する。近くに対象のGimmickTriggerがある場合、かつ向きが正しい場合のみ true。
    /// </summary>
    public override bool CanExecute(ItemData item)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        // ★ 1. 向きのチェック
        if (requiredDirection != -1)
        {
            var movement = player.GetComponent<GridMovement>();
            if (movement == null) return false;

            if (movement.GetDirection() != requiredDirection)
            {
                Debug.LogWarning($"[ExchangeItemEffect] 向きが正しくありません。 (必要: {requiredDirection}, 現在: {movement.GetDirection()})");
                return false;
            }
        }

        // ★ 2. コライダー範囲のチェック (既存ロジック)
        var triggers = Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            if (!trigger.IsPlayerNear) continue;

            var gimmick = trigger.GetGimmick<ItemExchangeGimmick>();
            if (gimmick != null)
            {
                if (string.IsNullOrEmpty(targetGimmickID) || gimmick.gimmickID == targetGimmickID)
                {
                    // 向きのチェックとコライダーのチェックの両方を通過
                    return true;
                }
            }
        }

        Debug.LogWarning($"[ExchangeItemEffect] GimmickTriggerのコライダー範囲外か、対象ギミックが見つかりません。");
        return false;
    }

    public override void Execute(ItemData usedItem)
    {
        // ... (Execute の中身は変更なし) ...

        bool success = TryInvokeNearbyGimmick<ItemExchangeGimmick>(gimmick =>
        {
            if (!string.IsNullOrEmpty(targetGimmickID) && gimmick.gimmickID != targetGimmickID)
                return;

            if (gimmick.ExecuteExchange())
            {
                InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            }
        });

        if (!success)
        {
            Debug.LogWarning($"[ExchangeItemEffect] ギミックの実行中に失敗しました。アイテムは削除されませんでした。");
        }
    }
}