using System.Collections.Generic;
using UnityEngine;

// GimmickBaseを継承し、アイテムの交換・生成とトリガーの無効化、ドア閉鎖を管理する
public class ItemExchangeGimmick : GimmickBase
{
    [Header("交換に成功した後に生成されるアイテム")]
    public ItemData rewardItemData;

    [Header("生成するアイテムの見た目オブジェクト (生成後表示)")]
    public GameObject rewardObject;

    [Header("交換成功時に無効化するGimmickTriggerのリスト")]
    public List<GimmickTrigger> triggersToDisable = new List<GimmickTrigger>();

    [Header("交換成功後に閉鎖するドア")]
    public DoorController doorToLock;

    /// <summary>
    /// アイテム交換処理を実行する
    /// </summary>
    /// <returns>交換に成功したら true</returns>
    public bool ExecuteExchange()
    {
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ExecuteExchangeが呼ばれました。");

        // 既に交換済み（currentStageが1以上）なら失敗
        if (this.currentStage >= 1)
        {
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ギミックは既に完了しています。実行をスキップします。");
            return false;
        }

        if (rewardItemData == null)
        {
            Debug.LogWarning($"[ItemExchangeGimmick: {gimmickID}] 報酬アイテムが設定されていません。失敗。");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError($"[ItemExchangeGimmick: {gimmickID}] InventoryManagerがNULLです。アイテムを追加できません。");
            return false;
        }

        // --- 成功処理 ---

        // 1. トリガーを無効化
        DisableTriggers();

        // 2. ドアを閉鎖 (currentStageを0に設定)
        if (doorToLock != null)
        {
            // DoorControllerの LoadProgress(0) を呼び出し、状態を閉鎖（Stage 0）に設定
            doorToLock.LoadProgress(0);
            Debug.Log($"[ItemExchangeGimmick] {doorToLock.gameObject.name} を閉鎖しました (Stage 0)。");
        }

        // 3. インベントリに報酬アイテムを追加
        InventoryManager.Instance.AddItem(rewardItemData);
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardItemData.name} ({rewardItemData.itemID}) をインベントリに追加しました。");

        // 4. 進行度を更新（完了とする）
        this.currentStage = 1;

        // 5. 報酬アイテムの見た目を表示（設定されている場合）
        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardObject.name} を表示しました。");
        }

        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] 交換完了: {rewardItemData.itemName} を入手処理完了。");

        return true;
    }

    /// <summary>
    /// 設定されたトリガーを無効化する
    /// </summary>
    private void DisableTriggers()
    {
        foreach (var trigger in triggersToDisable)
        {
            if (trigger != null)
            {
                // GimmickTriggerコンポーネントとコライダーを無効化
                trigger.enabled = false;
                var collider = trigger.GetComponent<Collider2D>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
                Debug.Log($"[ItemExchangeGimmick] トリガーを無効化: {trigger.gameObject.name}");
            }
        }
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        // ロード時にギミックが完了している場合、報酬の見た目とトリガーの状態を復元
        if (this.currentStage >= 1)
        {
            if (rewardObject != null)
            {
                rewardObject.SetActive(true);
            }
            // 完了状態ならトリガーも無効化を復元
            DisableTriggers();
        }
    }
}