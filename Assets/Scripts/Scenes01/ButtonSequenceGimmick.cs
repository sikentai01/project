using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ButtonSequenceGimmick : GimmickBase
{
    [Header("ギミック設定")]
    public ItemData rewardItemData;

    [Header("初期/リセット時に表示する会話ID")]
    public string initialConversationId;

    [Header("正解のボタン順序 (インデックス 0～3)")]
    public List<int> correctSequence = new List<int> { 0, 1, 2, 3 };

    private List<int> inputSequence = new List<int>();
    private GimmickCanvasController canvasController;

    private bool isPlayerNear = false;
    private bool isActive = false; // 現在キャンバス操作中かどうか

    // =====================================================
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => BootLoader.HasBooted);

        canvasController = GimmickCanvasController.Instance;
        inputSequence.Clear();

        if (currentStage > 0)
            inputSequence = Enumerable.Repeat(0, currentStage).ToList();

        Debug.Log($"[Gimmick] {gimmickID} 初期化完了 stage={currentStage}");
    }


    private void Update()
    {
        if (!isPlayerNear) return;
        if (PauseMenu.isPaused) return;
        if (BootLoader.IsPlayerSpawning) return;

        // === Enterキー入力 ===
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isActive)
            {
                OpenCanvas();
            }
        }
    }

    private void OpenCanvas()
    {
        if (canvasController == null)
        {
            Debug.LogWarning("[ButtonSequenceGimmick] CanvasController が見つかりません。");
            return;
        }

        isActive = true;
        canvasController.ShowCanvas(this);
        DisplayConversationPage(initialConversationId);
    }

    // =====================================================
    //  ボタン押下イベント（CanvasControllerから呼ばれる）
    // =====================================================
    public void OnButtonClick(int clickedIndex)
    {
        if (currentStage >= correctSequence.Count)
        {
            CloseCanvas();
            return;
        }

        if (inputSequence.Count == currentStage)
        {
            int expectedIndex = correctSequence[currentStage];

            if (clickedIndex == expectedIndex)
            {
                inputSequence.Add(clickedIndex);
                currentStage++;

                if (currentStage == correctSequence.Count)
                {
                    CompleteGimmick();
                    return;
                }

                Debug.Log($"[Sequence] 正解。次のステップへ ({currentStage}/{correctSequence.Count})");
                DisplayConversationPage($"{initialConversationId}_{currentStage}");
            }
            else
            {
                // 不正解 → リセット
                inputSequence.Clear();
                currentStage = 0;
                Debug.Log($"[Sequence] 不正解。リセット。");
                DisplayConversationPage(initialConversationId);
            }
        }
    }

    private void CompleteGimmick()
    {
        if (rewardItemData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(rewardItemData);
            Debug.Log($"[Sequence] ギミック完了！報酬 {rewardItemData.itemName} を入手しました。");
        }

        DisplayConversationPage($"{initialConversationId}_Completed");
        CloseCanvas();
    }

    private void CloseCanvas()
    {
        if (canvasController != null)
            canvasController.HideCanvas();

        isActive = false;
    }

    // =====================================================
    //  会話表示（DialogueCore / ConversationHub 経由）
    // =====================================================
    private void DisplayConversationPage(string conversationId)
    {
        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire(conversationId);
        }
        else
        {
            Debug.LogWarning($"[Sequence] ConversationHub が見つかりません。会話ID: {conversationId}");
        }
    }

    // =====================================================
    //  プレイヤー接近判定（ギミック自身で管理）
    // =====================================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (isActive)
                CloseCanvas();
        }
    }

    // =====================================================
    //  セーブ / ロード（共通化）
    // =====================================================
    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        inputSequence.Clear();
        if (stage > 0)
            inputSequence = Enumerable.Repeat(0, stage).ToList();

        Debug.Log($"[Gimmick] {gimmickID} LoadProgress: {stage}");
    }
}
