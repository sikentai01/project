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

    [Header("アイテム使用時に開始する会話ID（空欄なら会話なしで即実行）")]
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
                // プレイヤーの向きが違う場合、警告を出さずに false
                return false;
            }
        }

        // 2. ギミックの存在チェック
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

        if (gimmick == null)
        {
            Debug.LogWarning("[ExchangeItemEffect] ギミックが見つかりません。実行できません。");
            return;
        }

        bool hasConversation = !string.IsNullOrEmpty(conversationId);

        if (hasConversation)
        {
            // --- 会話ありの場合: 会話開始と終了待ち ---

            if (DialogueCore.Instance == null)
            {
                Debug.LogWarning("[ExchangeItemEffect] DialogueCoreが見つかりません。会話をスキップし、直接実行します。");
                // Coreがない場合は会話なしと同じ処理へフォールバック
                ExecuteGimmickAndFinish(gimmick, usedItem);
                return;
            }

            // 1. 以前のリスナーを解除
            if (activeCallbacks.ContainsKey(conversationId))
            {
                DialogueCore.Instance.OnConversationEnded -= activeCallbacks[conversationId];
                activeCallbacks.Remove(conversationId);
            }

            // 2. 会話終了時に実行するアクションを定義
            Action<string> onConvEnd = null;
            onConvEnd = (finishedId) =>
            {
                if (finishedId != conversationId) return;
                ExecuteGimmickAndFinish(gimmick, usedItem); // ギミック実行

                // リスナー解除
                DialogueCore.Instance.OnConversationEnded -= onConvEnd;
                activeCallbacks.Remove(conversationId);
            };

            // 3. イベントリスナーを登録し、会話を開始
            DialogueCore.Instance.OnConversationEnded += onConvEnd;
            activeCallbacks.Add(conversationId, onConvEnd);

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
        else
        {
            // --- 会話なしの場合: 即座にギミック実行 ---
            ExecuteGimmickAndFinish(gimmick, usedItem);
        }
    }

    // 会話なし/会話終了後に実行される共通処理
    private void ExecuteGimmickAndFinish(ItemExchangeGimmick gimmick, ItemData usedItem)
    {
        if (gimmick.ExecuteExchange())
        {
            // SE再生
            if (SoundManager.Instance != null && successSeClip != null)
            {
                SoundManager.Instance.PlaySE(successSeClip);
                Debug.Log($"[ExchangeItemEffect] ギミック成功 SE再生: {successSeClip.name}");
            }

            // アイテム削除
            InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
        }
        Debug.Log("[ExchangeItemEffect] ギミック実行処理完了。");
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