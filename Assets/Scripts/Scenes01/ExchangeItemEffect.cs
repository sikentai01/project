using UnityEngine;

[CreateAssetMenu(menuName = "Gimmick/Exchange Item Effect", fileName = "ExchangeItemEffect")]
public class ExchangeItemEffect : GimmickEffectBase
{
    [Header("操作するギミックID (任意：近くに複数のギミックがある場合)")]
    public string targetGimmickID;

    /// <summary>
    /// アイテムスロットからの使用を許可するため、常に true を返す
    /// </summary>
    public override bool CanExecute(ItemData item)
    {
        // 常に使用可能とする。コライダーチェックは Execute 内で行う。
        return true;
    }

    public override void Execute(ItemData usedItem)
    {
        // TryInvokeNearbyGimmickで、近くの ItemExchangeGimmick を探して実行する
        bool success = TryInvokeNearbyGimmick<ItemExchangeGimmick>(gimmick =>
        {
            // ギミックIDが設定されていればチェック
            if (!string.IsNullOrEmpty(targetGimmickID) && gimmick.gimmickID != targetGimmickID)
                return;

            // アイテム交換処理を実行
            if (gimmick.ExecuteExchange())
            {
                // 交換に成功したら、使用アイテムをインベントリからIDを使って削除
                InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            }
        });

        if (!success)
        {
            // ギミックが見つからなかった、または IsPlayerNear の範囲外だった場合のメッセージ
            Debug.LogWarning($"[ExchangeItemEffect] 近くに対象のギミックが見つかりません。アイテムは削除されませんでした。");
        }
    }
}