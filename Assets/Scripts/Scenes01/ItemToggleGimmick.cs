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

    // ★★★ 追加フィールド ★★★
    [Header("所持時にStage 0にリセットするアイテムID")]
    public string resetIfHasItemID = "";
    // ★★★ ここまで ★★★

    private GimmickTrigger associatedTrigger; // 監視対象のトリガー

    // Enterキー入力の有効範囲チェックに使う
    private bool IsPlayerNear => associatedTrigger != null && associatedTrigger.IsPlayerNear;
    private bool isLoaded = false;

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
        // currentStageに基づいて初期状態を設定（LoadProgressで上書きされる可能性あり）
        UpdateVisualState();

        // ★★★ ロード後の状態チェックを StartCoroutine で実行 ★★★
        if (gimmickID != "") // IDが設定されているギミックのみチェック
        {
            StartCoroutine(CheckInventoryAndApplyLoadState());
        }
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
        // LoadProgressでは、セーブデータの値 (stage) をそのまま適用するのみ
        // リセット処理は CheckInventoryAndApplyLoadState() に任せる
        this.currentStage = stage;
        UpdateVisualState();
        isLoaded = true;
    }

    // ★★★ ItemTriggerと同様の遅延チェックロジック（ロード後の矛盾解消） ★★★
    private IEnumerator CheckInventoryAndApplyLoadState()
    {
        // InventoryManager が初期化されるまで待機
        while (InventoryManager.Instance == null)
        {
            yield return null;
        }

        // InventoryManagerがセーブデータロードを完了するのを待つための猶予
        yield return new WaitForSeconds(0.1f);

        // 既に LoadProgress でセーブデータが適用されているはずなので、その状態をチェック

        string itemID = resetIfHasItemID;
        if (!string.IsNullOrEmpty(itemID) && InventoryManager.Instance != null)
        {
            bool isItemInInventory = InventoryManager.Instance.HasItem(itemID);

            if (isItemInInventory)
            {
                // アイテムを持っている場合は、セーブデータの値が何であれ Stage 0 に強制リセット
                this.currentStage = 0;
                UpdateVisualState();
                Debug.Log($"[ItemToggleGimmick:{gimmickID}] **遅延チェック完了**: アイテム所持により Stage 0 にリセットしました。");
            }
        }
    }
}