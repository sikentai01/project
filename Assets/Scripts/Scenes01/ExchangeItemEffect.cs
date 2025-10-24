using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Gimmick/Exchange Item Effect", fileName = "ExchangeItemEffect")]
public class ExchangeItemEffect : GimmickEffectBase
{
    [Header("操作するギミックID (任意：近くに複数のギミックがある場合)")]
    public string targetGimmickID;

    [Header("必要な向き (0=下, 1=左, 2=右, 3=上, -1=制限なし)")]
    public int requiredDirection = -1;

    // ギミック起動に使用する会話ID
    [Header("アイテム使用時に開始する会話ID")]
    public string conversationId = "";

    [Header("ギミック成功時に再生するSE")]
    public AudioClip successSeClip;

    private static Dictionary<string, Action<string>> activeCallbacks = new Dictionary<string, Action<string>>();

    // CanExecute() のロジックは変更なし

    public override bool CanExecute(ItemData item)
    {
        var player = UnityEngine.GameObject.FindGameObjectWithTag("Player");
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
        var triggers = UnityEngine.Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
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
        var gimmick = FindNearbyGimmick<ItemExchangeGimmick>();
        if (gimmick == null || DialogueCore.Instance == null || string.IsNullOrEmpty(conversationId))
        {
            Debug.LogWarning("[ExchangeItemEffect] ギミック/Core/会話IDのいずれかが不足しています。");
            return;
        }

        if (activeCallbacks.ContainsKey(conversationId))
        {
            DialogueCore.Instance.OnConversationEnded -= activeCallbacks[conversationId];
            activeCallbacks.Remove(conversationId);
        }

        // 3. 会話終了時に実行するアクションを定義
        Action<string> onConvEnd = null;
        onConvEnd = (finishedId) =>
        {
            if (finishedId != conversationId) return;

            // ギミック起動、SE再生、アイテム削除
            if (gimmick.ExecuteExchange())
            {
                // ★★★ CS0571 エラー修正箇所 ★★★
                if (SoundManager.Instance != null && successSeClip != null)
                {
                    SoundManager.Instance.PlaySE(successSeClip);
                    Debug.Log($"[ExchangeItemEffect] 会話終了後 SE再生: {successSeClip.name}");
                }

                // アイテム削除
                InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            }

            // リスナー解除
            DialogueCore.Instance.OnConversationEnded -= onConvEnd;
            activeCallbacks.Remove(conversationId);
            Debug.Log($"[ExchangeItemEffect] 会話終了処理完了 ({conversationId})");
        };

        // 4. イベントリスナーを登録
        DialogueCore.Instance.OnConversationEnded += onConvEnd;
        activeCallbacks.Add(conversationId, onConvEnd);

        // 5. 会話を開始 (ConversationHub経由)
        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire(conversationId);
            Debug.Log($"[ExchangeItemEffect] 会話開始: {conversationId}");
        }
        else
        {
            Debug.LogWarning("[ExchangeItemEffect] ConversationHubが見つからないため会話を開始できません。");
        }
    }

    // 近くのギミックを見つけるヘルパー
    private T FindNearbyGimmick<T>() where T : GimmickBase
    {
        var triggers = UnityEngine.Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            if (!trigger.IsPlayerNear) continue;

            var gimmick = trigger.GetGimmick<T>();
            if (gimmick != null)
            {
                if (string.IsNullOrEmpty(targetGimmickID) || gimmick.gimmickID == targetGimmickID)
                {
                    return gimmick;
                }
            }
        }
        return null;
    }
}