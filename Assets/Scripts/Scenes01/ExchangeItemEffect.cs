using UnityEngine;

[CreateAssetMenu(menuName = "Gimmick/Exchange Item Effect", fileName = "ExchangeItemEffect")]
public class ExchangeItemEffect : GimmickEffectBase
{
    [Header("操作するギミックID (任意：近くに複数のギミックがある場合)")]
    public string targetGimmickID;

    [Header("必要な向き (0=下, 1=左, 2=右, 3=上, -1=制限なし)")]
    public int requiredDirection = -1;

    [Header("ギミック成功時に再生するSE")]
    public AudioClip successSeClip; // ★ 新規追加フィールド

    /// <summary>
    /// アイテムが使用可能か判定する。向きとコライダー範囲内をチェック。
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
                Debug.LogWarning($"[ExchangeItemEffect] 向きが正しくありません。 (必要: {requiredDirection}, 現在: {movement.GetDirection()})");
                return false;
            }
        }

        // 2. コライダー範囲のチェック
        var triggers = Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            if (!trigger.IsPlayerNear) continue;

            var gimmick = trigger.GetGimmick<ItemExchangeGimmick>();
            if (gimmick != null)
            {
                if (string.IsNullOrEmpty(targetGimmickID) || gimmick.gimmickID == targetGimmickID)
                {
                    return true;
                }
            }
        }

        Debug.LogWarning($"[ExchangeItemEffect] GimmickTriggerのコライダー範囲外か、対象ギミックが見つかりません。");
        return false;
    }

    public override void Execute(ItemData usedItem)
    {
        bool success = TryInvokeNearbyGimmick<ItemExchangeGimmick>(gimmick =>
        {
            if (!string.IsNullOrEmpty(targetGimmickID) && gimmick.gimmickID != targetGimmickID)
                return;

            // アイテム交換処理を実行
            if (gimmick.ExecuteExchange())
            {
                // ★ SE再生処理の追加 ★
                if (SoundManager.Instance != null && successSeClip != null)
                {
                    SoundManager.Instance.PlaySE(successSeClip);
                    Debug.Log($"[ExchangeItemEffect] SE再生: {successSeClip.name}");
                }

                // 成功した場合のみアイテムを削除
                InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            }
        });

        if (!success)
        {
            Debug.LogWarning($"[ExchangeItemEffect] 近くに対象のギミックが見つかりません。アイテムは削除されませんでした。");
        }
    }
}