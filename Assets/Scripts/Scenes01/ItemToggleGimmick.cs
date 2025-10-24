using UnityEngine;
using System.Collections;

/// <summary>
/// アイテム使用とEnterキーで状態をトグルし、アイテムの消費・復元を行うギミック
/// Stage 0: 待機状態 (オブジェクト非表示、アイテムが手元にある状態)
/// Stage 1: 設置状態 (オブジェクト表示、アイテムが消費された状態)
/// </summary>
public class ItemToggleGimmick : GimmickBase
{
    [Header("このギミックでトグルするアイテムデータ")]
    public ItemData toggleItemData;

    [Header("表示/非表示を切り替えるターゲット")]
    public GameObject targetObject;

    [Header("対応するGimmickTriggerのID")] // Enterキー入力の範囲を特定するために使用
    public string triggerIDToMonitor;

    private GimmickTrigger associatedTrigger; // 監視対象のトリガー
    private bool isInitialized = false;

    private void Awake()
    {
        // プレイヤーの位置確認のために Awake でトリガーを探す
        if (!string.IsNullOrEmpty(triggerIDToMonitor))
        {
            var triggers = FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
            foreach (var trigger in triggers)
            {
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
        UpdateVisualState();
    }

    private void Update()
    {
        // プレイヤーの接近判定を associatedTrigger 経由で行う
        bool isPlayerInZone = associatedTrigger != null && associatedTrigger.IsPlayerNear;

        // 設置状態 (Stage 1) でプレイヤーが範囲内にいる場合に Enter キー入力を検出
        if (currentStage == 1 && isPlayerInZone && Input.GetKeyDown(KeyCode.Return))
        {
            // 設置状態から待機状態へトグルする
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
        if (currentStage != 0) return false;

        if (targetObject == null || toggleItemData == null)
        {
            Debug.LogWarning("[ItemToggleGimmick] ターゲットまたはアイテムが未設定です。");
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
        if (currentStage != 1) return;

        if (targetObject == null || toggleItemData == null)
        {
            Debug.LogWarning("[ItemToggleGimmick] ターゲットまたはアイテムが未設定です。");
            return;
        }

        // ギミック解除とアイテム復元
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
        if (targetObject == null) return;

        // Stage 1 なら表示、Stage 0 なら非表示
        targetObject.SetActive(currentStage == 1);
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);
        UpdateVisualState();
    }
}