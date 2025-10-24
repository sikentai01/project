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


    private void Start()
    {
        // currentStageに基づいて初期状態を設定
        // LoadProgressが呼ばれる前に、Inspectorで設定されたcurrentStage(通常は0)で初期化される
        // LoadProgressが呼ばれたらその値で上書きされ、UpdateVisualState()が呼ばれる
        UpdateVisualState();
    }


    /// <summary>
    /// アイテム交換処理を実行する
    /// </summary>
    /// <returns>交換に成功したら true</returns>
    public bool ExecuteExchange()
    {
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ExecuteExchangeが呼ばれました。");

        // 1. 既に完了済み（currentStageが1以上）かつ、繰り返し不可(isRepeatable=false)なら失敗
        if (currentStage >= 1 && !isRepeatable)
        {
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] 既に完了済みで繰り返し不可のため、失敗。");
            return false;
        }

        // 2. 報酬アイテムをインベントリに追加
        if (rewardItemData != null)
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.AddItem(rewardItemData);
                Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardItemData.itemName} をインベントリに追加しました。");
            }
            else
            {
                Debug.LogWarning("[ItemExchangeGimmick] InventoryManager.Instanceがnullです。");
            }
        }
        else
        {
            Debug.LogWarning("[ItemExchangeGimmick] rewardItemDataが設定されていません。");
        }


        // 3. 初回実行時 (Stage 0) のみサイドエフェクトを実行
        if (currentStage == 0)
        {
            // A. トリガーの無効化
            DisableTriggers();

            // B. ドアの施錠
            if (doorToLock != null)
            {
                doorToLock.currentStage = 0; // ドアを閉じる (DoorController側で見た目を更新する想定)
                Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {doorToLock.gameObject.name} を施錠しました。");
            }

            // C. ゲームオーバーコントローラーをアクティブ化
            if (gameOverControllerObject != null)
            {
                gameOverControllerObject.SetActive(true);
                Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {gameOverControllerObject.name} をアクティブにしました。");
            }

            // D. 進行度を更新（完了とする）
            this.currentStage = 1;
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] 初回完了(Stage 1)に設定しました。");
        }


        // 4. 報酬アイテムの見た目を表示（設定されている場合）
        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardObject.name} を表示しました。");
        }

        // Stage 1 になった後の繰り返し交換時は currentStage の変更はなし

        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] 交換処理成功。");

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

    /// <summary>
    /// 現在の進行段階に基づきオブジェクトの状態を更新
    /// </summary>
    private void UpdateVisualState()
    {
        // 報酬オブジェクトの表示/非表示を currentStage に基づいて決定
        if (rewardObject != null)
        {
            // Stage 1以上なら表示
            rewardObject.SetActive(currentStage >= 1);
        }

        // その他の視覚的な変化が必要であればここに追加
    }

    // =====================================================
    // セーブ・ロード
    // =====================================================

    public override void LoadProgress(int stage)
    {
        // ★★★ 修正点: 親クラス (GimmickBase) の実装を呼び出し、セーブデータ通りの進行段階を復元する ★★★
        base.LoadProgress(stage);

        // ロードされた進行段階に基づいて見た目を更新
        UpdateVisualState();

        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ロード完了: セーブ値 {stage} を復元しました。");
    }
}