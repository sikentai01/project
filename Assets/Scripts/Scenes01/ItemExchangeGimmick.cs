using System.Collections.Generic;
using UnityEngine;

// ギミックの基本クラスを継承
public class ItemExchangeGimmick : GimmickBase
{
    [Header("交換に成功した後に生成されるアイテム")]
    public ItemData rewardItemData;

    [Header("生成するアイテムの見た目オブジェクト (生成後表示)")]
    public GameObject rewardObject;

    // ★ 追加フィールド：繰り返し可能か
    [Header("アイテム交換を何度でも可能にするか")]
    public bool isRepeatable = false;

    // 以下の特殊なサイドエフェクトは currentStage == 0 の時のみ実行
    [Header("交換成功時に無効化するGimmickTriggerのリスト (初回のみ)")]
    public List<GimmickTrigger> triggersToDisable = new List<GimmickTrigger>();

    [Header("交換成功後に閉鎖するドア (初回のみ)")]
    public DoorController doorToLock;

    [Header("交換成功後にアクティブにするGameOverコントローラー (初回のみ)")]
    public GameObject gameOverControllerObject;


    /// <summary>
    /// アイテム交換処理を実行する
    /// </summary>
    /// <returns>交換に成功したら true</returns>
    public bool ExecuteExchange()
    {
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ExecuteExchangeが呼ばれました。");

        // 既に完了済み（currentStageが1以上）かつ、繰り返し不可(isRepeatable=false)なら失敗
        if (!isRepeatable && this.currentStage >= 1)
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

        // 1. ドアを閉鎖 (currentStageを0に設定) - 毎回実行する
        if (doorToLock != null)
        {
            // DoorControllerの LoadProgress(0) を呼び出し、状態を閉鎖（Stage 0）に設定
            doorToLock.LoadProgress(0);
            Debug.Log($"[ItemExchangeGimmick] {doorToLock.gameObject.name} を閉鎖しました (Stage 0)。");
        }

        // 2. 報酬アイテムを追加 (これも毎回実行)
        InventoryManager.Instance.AddItem(rewardItemData);
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardItemData.name} ({rewardItemData.itemID}) をインベントリに追加しました。");


        // 3. 初回のみ実行する特殊なサイドエフェクトの処理
        if (this.currentStage == 0) // currentStage が 0 (初期状態) の場合のみ実行
        {
            // A. トリガーを無効化
            DisableTriggers();

            // B. ゲームオーバー処理の実行
            if (gameOverControllerObject != null)
            {
                gameOverControllerObject.SetActive(true);
                Debug.Log("[ItemExchangeGimmick] 初回実行 → ゲームオーバー起動");
            }

            // C. 進行度を更新（完了とする）：これによりセーブされ、次回以降のこのブロックはスキップされる
            this.currentStage = 1;
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] 初回完了(Stage 1)に設定しました。");
        }


        // 4. 報酬アイテムの見た目を表示（設定されている場合）
        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardObject.name} を表示しました。");
        }

        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] 交換完了: {rewardItemData.itemName} の入手処理完了。");

        return true;
    }

    /// <summary>
    /// 設定されたトリガーを無効化する
    /// </summary>
    private void DisableTriggers()
    {
        // ... (DisableTriggers の中身は変更なし) ...
        foreach (var trigger in triggersToDisable)
        {
            if (trigger != null)
            {
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
        if (this.currentStage >= 1 && !isRepeatable) // 繰り返し不可のギミックのみロード時状態復元
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