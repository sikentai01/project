using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ButtonSequenceGimmick : GimmickBase
{
    [Header("ギミック設定")]
    public ItemData rewardItemData;

    [Header("初期/リセット時に表示する会話ID")]
    public string initialConversationId;

    [Header("正解のボタン順序 (インデックス 0～3)")]
    [Tooltip("クリックすべきボタンのインデックスを順番に設定")]
    public List<int> correctSequence = new List<int> { 0, 1, 2, 3 };

    private List<int> inputSequence = new List<int>();
    private GimmickCanvasController canvasController;

    private bool initialized = false;

    // -----------------------------------------------------
    private void Start()
    {
        StartCoroutine(WaitForBootstrapAndCanvas());
    }

    private System.Collections.IEnumerator WaitForBootstrapAndCanvas()
    {
        yield return new WaitUntil(() =>
            BootLoader.HasBooted &&
            GimmickCanvasController.Instance != null);

        canvasController = GimmickCanvasController.Instance;
        InitializeGimmick();
        initialized = true;
    }

    private void InitializeGimmick()
    {
        if (currentStage == 0)
        {
            inputSequence.Clear();
            DisplayConversationPage(initialConversationId);
        }
        else if (currentStage < correctSequence.Count)
        {
            inputSequence = Enumerable.Repeat(0, currentStage).ToList();
            DisplayConversationPage($"{initialConversationId}_{currentStage}");
        }
        else
        {
            inputSequence = Enumerable.Repeat(0, correctSequence.Count).ToList();
            DisplayConversationPage($"{initialConversationId}_Completed");
        }
    }

    // -----------------------------------------------------
    // UI操作用関数（ボタン押下時に呼ばれる）
    public void OnButtonClick(int clickedIndex)
    {
        if (!initialized) return;
        if (currentStage >= correctSequence.Count) return; // 完了済み

        int expected = correctSequence[currentStage];
        if (clickedIndex == expected)
        {
            inputSequence.Add(clickedIndex);
            currentStage++;

            if (currentStage == correctSequence.Count)
            {
                CompleteGimmick();
                return;
            }

            Debug.Log($"[Sequence] 正解: {currentStage}/{correctSequence.Count}");
            DisplayConversationPage($"{initialConversationId}_{currentStage}");
        }
        else
        {
            ResetSequence();
        }
    }

    // -----------------------------------------------------
    private void ResetSequence()
    {
        inputSequence.Clear();
        currentStage = 0;
        Debug.Log("[Sequence] 不正解 → リセット");
        DisplayConversationPage(initialConversationId);
    }

    private void CompleteGimmick()
    {
        Debug.Log("[Sequence] ギミック完了！");

        if (rewardItemData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(rewardItemData);
            Debug.Log($"[Sequence] 報酬 {rewardItemData.itemName} を入手！");
        }

        DisplayConversationPage($"{initialConversationId}_Completed");
        canvasController?.HideCanvas();
    }

    // -----------------------------------------------------
    // セーブ・ロード
    public override GimmickSaveData SaveProgress()
    {
        var data = base.SaveProgress();
        data.stage = currentStage;
        return data;
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);
        InitializeGimmick();
    }

    // -----------------------------------------------------
    // 会話呼び出し
    public void DisplayConversationPage(string conversationId)
    {
        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire(conversationId);
        }
        else
        {
            Debug.LogWarning($"[Sequence] ConversationHubが見つかりません。ID: {conversationId}");
        }
    }
}
