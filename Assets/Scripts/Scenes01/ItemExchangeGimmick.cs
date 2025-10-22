using UnityEngine;

// GimmickBaseを継承し、アイテムの交換・生成を管理する
public class ItemExchangeGimmick : GimmickBase
{
    [Header("交換に成功した後に生成されるアイテム")]
    public ItemData rewardItemData;

    [Header("生成するアイテムの見た目オブジェクト (生成後表示)")]
    public GameObject rewardObject;

    /// <summary>
    /// アイテム交換処理を実行する
    /// </summary>
    /// <returns>交換に成功したら true</returns>
    public bool ExecuteExchange()
    {
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ExecuteExchangeが呼ばれました。"); // ★ 追加ログ

        // 既に交換済み（currentStageが1以上）なら失敗
        if (this.currentStage >= 1)
        {
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] ギミックは既に完了しています。実行をスキップします。"); // ★ 追加ログ
            return false;
        }

        if (rewardItemData == null)
        {
            Debug.LogWarning($"[ItemExchangeGimmick: {gimmickID}] 報酬アイテムが設定されていません。失敗。"); // ★ 追加ログ
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError($"[ItemExchangeGimmick: {gimmickID}] InventoryManager.InstanceがNULLです。アイテムを追加できません。"); // ★ 追加ログ
            return false;
        }

        // インベントリに報酬アイテムを追加
        InventoryManager.Instance.AddItem(rewardItemData);
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardItemData.name} ({rewardItemData.itemID}) をインベントリに追加しました。"); // ★ 追加ログ

        // 進行度を更新（完了とする）
        this.currentStage = 1;

        // 報酬アイテムの見た目を表示（設定されている場合）
        if (rewardObject != null)
        {
            rewardObject.SetActive(true);
            Debug.Log($"[ItemExchangeGimmick: {gimmickID}] {rewardObject.name} を表示しました。");
        }

        // ギミック完了ログ
        Debug.Log($"[ItemExchangeGimmick: {gimmickID}] 交換完了: {rewardItemData.itemName} を入手処理完了。");

        return true;
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        // ロード時にギミックが完了している場合、報酬の見た目を復元
        if (this.currentStage >= 1 && rewardObject != null)
        {
            rewardObject.SetActive(true);
        }
    }
}