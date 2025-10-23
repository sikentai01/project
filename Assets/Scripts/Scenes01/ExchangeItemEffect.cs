using UnityEngine;

[CreateAssetMenu(menuName = "Gimmick/Exchange Item Effect", fileName = "ExchangeItemEffect")]
public class ExchangeItemEffect : GimmickEffectBase
{
    [Header("操作するギミックID (任意：近くに複数のギミックがある場合)")]
    public string targetGimmickID;

    /// <summary>
    /// アイテムが使用可能か判定する。近くに対象のGimmickTriggerがある場合のみ true。
    /// </summary>
    public override bool CanExecute(ItemData item)
    {
        // シーン内のすべてのGimmickTriggerを探す
        var triggers = Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            // 1. プレイヤーがトリガーの範囲内にいるか？
            if (!trigger.IsPlayerNear) continue;

            // 2. このトリガーが ItemExchangeGimmick を保持しているか？
            var gimmick = trigger.GetGimmick<ItemExchangeGimmick>();
            if (gimmick != null)
            {
                // 3. ギミックIDの指定があれば、それも一致しているか？
                if (string.IsNullOrEmpty(targetGimmickID) || gimmick.gimmickID == targetGimmickID)
                {
                    // すべての条件を満たせば、アイテム使用を許可
                    return true;
                }
            }
        }

        // 近くに対象のギミックが見つからない場合は使用不可
        Debug.LogWarning($"[ExchangeItemEffect] GimmickTriggerのコライダー範囲外か、対象ギミックが見つかりません。");
        return false;
    }

    public override void Execute(ItemData usedItem)
    {
        // CanExecute() が true を返しているため、Executeが実行される = 近くに対象ギミックがあるはず

        // GimmickEffectBaseの共通メソッドでギミックを実行
        bool success = TryInvokeNearbyGimmick<ItemExchangeGimmick>(gimmick =>
        {
            // ギミックIDのチェック
            if (!string.IsNullOrEmpty(targetGimmickID) && gimmick.gimmickID != targetGimmickID)
                return;

            // アイテム交換処理を実行
            if (gimmick.ExecuteExchange())
            {
                // 成功した場合のみアイテムを削除
                InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            }
        });

        if (!success)
        {
            // CanExecute()がtrueでも、何らかの理由でExecuteが失敗した場合は、アイテムは削除されずに残る
            Debug.LogWarning($"[ExchangeItemEffect] ギミックの実行中に失敗しました。アイテムは削除されませんでした。");
        }
    }
}