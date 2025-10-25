using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// アイテム使用とEnterキーで状態をトグルし、アイテムの消費・復元を行うギミック
/// Stage 0: 待機状態 (オブジェクトがOFF)
/// Stage 1: 設置状態 (オブジェクトがON)
/// </summary>
public class ItemToggleGimmick : GimmickBase
{
    [Header("このギミックでトグルするアイテムデータ")]
    public ItemData toggleItemData;

    [Header("表示/非表示を切り替えるオブジェクト群")]
    public GameObject[] targetObjects; // Stage 1でONになるグループ

    [Header("表示/非表示を逆に切り替えるオブジェクト群 (Stage 1でOFF)")]
    public GameObject[] offObjects; // Stage 1でOFFになるグループ

    [Header("対応するGimmickTriggerのID")] // Enterキー入力の範囲を特定するために使用
    public string triggerIDToMonitor;

    // ★ リセット機能のフィールドは今回は無視して、ロジックのみ修正します

    private GimmickTrigger associatedTrigger; // 監視対象のトリガー

    // Enterキー入力の有効範囲チェックに使う
    private bool IsPlayerNear => associatedTrigger != null && associatedTrigger.IsPlayerNear;
    // private bool isLoaded = false; // 不要になったため削除

    private void Awake()
    {
        // プレイヤーの位置確認のために Awake でトリガーを探す
        if (!string.IsNullOrEmpty(triggerIDToMonitor))
        {
            var triggers = FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
            foreach (var trigger in triggers)
            {
                // GimmickTriggerはGimmickIDを持っています
                if (trigger.gimmickID == triggerIDToMonitor)
                {
                    associatedTrigger = trigger;
                    break;
                }
            }
        }

        if (associatedTrigger == null && !string.IsNullOrEmpty(triggerIDToMonitor))
        {
            Debug.LogWarning($"[ItemToggleGimmick] ID:{triggerIDToMonitor} のGimmickTriggerが見つかりません。Enterキー入力が無効になります。");
        }
    }

    private void Start()
    {
        // currentStageに基づいて初期状態を設定（LoadProgressで上書きされる可能性あり）
        UpdateVisualState();

        // LoadProgressの後にInventoryManagerチェックが必要なら、ここでStartCoroutineを呼ぶ
    }

    private void Update()
    {
        // 設置状態 (Stage 1) でプレイヤーが範囲内にいる場合に Enter キー入力を検出
        if (currentStage == 1 && IsPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // 設置状態から待機状態へトグルする (アイテム復元＆非表示)
            TryRestoreItemAndHide();
        }
    }

    // =====================================================
    // 外部からの操作 (ItemEffect.Executeから呼ばれる)
    // =====================================================

    /// <summary>
    /// アイテムを使用してギミックを設置する (Stage 0 → 1)
    /// </summary>
    public bool TryPlaceAndConsumeItem()
    {
        if (currentStage != 0) return false; // 既に設置済みの場合は失敗

        if (targetObjects.Length == 0 || toggleItemData == null)
        {
            Debug.LogWarning("[ItemToggleGimmick] ターゲットオブジェクトまたはアイテムが未設定です。");
            return false;
        }

        // 設置成功
        this.currentStage = 1;
        UpdateVisualState();
        Debug.Log($"[ItemToggleGimmick] {toggleItemData.itemName} を使用して設置 (Stage 1)。");
        return true;
    }

    // =====================================================
    // 内部からの操作 (Update/Enterキーから呼ばれる)
    // =====================================================

    /// <summary>
    /// Enterキーを押してギミックを解除し、アイテムを復元する (Stage 1 → 0)
    /// </summary>
    private void TryRestoreItemAndHide()
    {
        if (currentStage != 1) return; // 設置済みでなければ失敗

        if (toggleItemData == null)
        {
            Debug.LogWarning("[ItemToggleGimmick] トグルアイテムが未設定です。アイテム復元できません。");
            return;
        }

        // ギミック解除とアイテム復元
        // ※ ここでのAddItemが InventoryManagerの HasItemチェックと連動し、矛盾が生じる場合は
        //    AddItem処理を TryRestoreItemAndHide の呼び出し元で制御する必要があります。
        InventoryManager.Instance.AddItem(toggleItemData);
        Debug.Log($"[ItemToggleGimmick] ギミック解除。{toggleItemData.itemName} を復元しました (Stage 0)。");

        this.currentStage = 0;
        UpdateVisualState();
    }

    // =====================================================
    // 状態更新とセーブ・ロード
    // =====================================================

    private void UpdateVisualState()
    {
        bool shouldBeActive = (currentStage == 1); // Stage 1ならON、0ならOFF

        // targetObjects: Stage 1でONになるグループ
        foreach (var obj in targetObjects)
        {
            if (obj != null)
                obj.SetActive(shouldBeActive);
        }

        // offObjects: Stage 1でOFFになるグループ
        foreach (var obj in offObjects)
        {
            if (obj != null)
                obj.SetActive(!shouldBeActive);
        }
    }

    public override void LoadProgress(int stage)
    {
        // ★★★ 修正点: 親クラス (GimmickBase) の実装を呼び出し、セーブデータ通りの進行段階を復元する ★★★
        base.LoadProgress(stage);

        // ロードされた進行段階に基づいて見た目を更新
        UpdateVisualState();

        Debug.Log($"[ItemToggleGimmick: {gimmickID}] ロード完了: セーブ値 {stage} を復元しました。");
    }
    // どこかのスクリプト内 (例)


}