using UnityEngine;
// 必要に応じて GimmickBase クラスが定義されている名前空間を追加
// using GimmickSystem; 

public class BreakWallTriggerGimmick : GimmickBase
{
    [Header("表示するオブジェクト (破壊された壁)")]
    public GameObject targetWallObject;

    // ★修正: 完了状態を管理するための private フラグを追加★
    private bool isGimmickDone = false;

    // ... (NeedsItem, CanUseItem, UseItem の部分は変更なし) ...
    public override bool NeedsItem => false;

    public override bool CanUseItem(ItemData item)
    {
        return false;
    }

    public override void UseItem(ItemData usedItem, ItemTrigger trigger)
    {
        Debug.Log("このギミックはアイテムでは作動しません。");
    }

    // -----------------------------------------------------
    // ギミックの起動処理
    // -----------------------------------------------------

    public override void StartGimmick(ItemTrigger trigger)
    {
        if (targetWallObject == null)
        {
            Debug.LogError("ターゲットとなる壁オブジェクトが設定されていません。");
            return;
        }

        // ★修正: IsCompleted の代わりに、ローカルのフラグでチェック★
        if (isGimmickDone)
        {
            Debug.Log("壁は既に破壊されています。");
            return;
        }

        // 壁オブジェクトを表示（破壊された状態にする）
        targetWallObject.SetActive(true);
        Debug.Log($"[Gimmick] {targetWallObject.name} を表示し、壁を破壊しました。");

        // ギミックを完了状態にする
        Complete(trigger);

        // ★追加: ギミック完了フラグを立てる★
        isGimmickDone = true;
    }
}