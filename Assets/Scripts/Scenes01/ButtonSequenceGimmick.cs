using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// ボタンの順序クリックとテキスト表示を制御するギミック
public class ButtonSequenceGimmick : GimmickBase
{
    [Header("ギミック設定")]
    public ItemData rewardItemData;

    [Header("初期/リセット時に表示する会話ID")]
    public string initialConversationId;

    [Header("正解のボタン順序 (インデックス 0～3)")]
    [Tooltip("クリックすべきボタンのインデックスを順番に設定")]
    public List<int> correctSequence = new List<int> { 0, 1, 2, 3 }; // 例: ボタン1→2→3→4

    // currentStage に現在の入力ステップを保存する
    private List<int> inputSequence = new List<int>();

    // 参照
    private GimmickCanvasController canvasController;

    // -----------------------------------------------------

    void Start()
    {
        // GimmickCanvasControllerをシーン内から検索
        canvasController = FindObjectOfType<GimmickCanvasController>();

        if (currentStage == 0)
        {
            inputSequence.Clear();
            // 初期テキストを表示
            DisplayConversationPage(initialConversationId);
        }
        else
        {
            // ロードまたは Stage 1 以上で開始された場合、完了状態のままにする
            inputSequence = Enumerable.Repeat(0, correctSequence.Count).ToList();
            // 完了したテキストを表示
            DisplayConversationPage($"{initialConversationId}_Completed");
        }
    }

    /// <summary>
    /// GimmickCanvasのボタンがクリックされたときに呼び出される
    /// </summary>
    /// <param name="clickedIndex">クリックされたボタンのインデックス (0～3)</param>
    public void OnButtonClick(int clickedIndex)
    {
        if (currentStage >= correctSequence.Count) return; // 既に完了済み

        if (inputSequence.Count == currentStage)
        {
            // 次の入力ステップをチェック
            int expectedIndex = correctSequence[currentStage];

            if (clickedIndex == expectedIndex)
            {
                // --- 正解処理 ---
                inputSequence.Add(clickedIndex);
                this.currentStage++; // ステージ進行（セーブ対象）

                if (currentStage == correctSequence.Count)
                {
                    // ギミック完了！
                    CompleteGimmick();
                    return;
                }

                // 次のテキストを表示 (ステップ数に対応したIDを想定)
                Debug.Log($"[Sequence] 正解。次のステップへ ({currentStage}/{correctSequence.Count})");
                DisplayConversationPage($"{initialConversationId}_{currentStage}");
            }
            else
            {
                // --- 不正解処理 ---
                inputSequence.Clear();
                this.currentStage = 0;
                Debug.Log($"[Sequence] 不正解。シーケンスをリセットしました。");
                DisplayConversationPage(initialConversationId); // 最初のテキストに戻る
            }
        }
    }

    /// <summary>
    /// ギミックが完了したときの最終処理
    /// </summary>
    private void CompleteGimmick()
    {
        if (rewardItemData != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(rewardItemData);
            Debug.Log($"[Sequence] ギミック完了！報酬 {rewardItemData.itemName} を入手しました。");
        }

        // 完了後のテキストを表示
        DisplayConversationPage($"{initialConversationId}_Completed");

        // Canvasを閉じる
        if (canvasController != null)
        {
            canvasController.HideCanvas();
        }

        // GimmickBaseのCompleteメソッドをここで呼んでも良いが、今回は完了状態をcurrentStageで管理
    }

    /// <summary>
    /// DialogueCoreの機能を使ってテキストパネルに会話を表示する
    /// </summary>
    /// <param name="conversationId">表示する会話ID</param>
    public void DisplayConversationPage(string conversationId)
    {
        // 既存の会話システム（ConversationHub）を起動する
        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire(conversationId);
        }
        else
        {
            Debug.LogWarning($"[Sequence] ConversationHubが見つかりません。会話ID: {conversationId} は表示されません。");
        }
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        // ロード後の復元処理
        inputSequence.Clear();
        if (currentStage > 0)
        {
            // 完了済みまたは途中まで進んでいた状態を再現
            inputSequence = Enumerable.Repeat(0, currentStage).ToList();
        }

        // 完了状態であれば、完了後のテキストを表示
        if (currentStage >= correctSequence.Count)
        {
            DisplayConversationPage($"{initialConversationId}_Completed");
        }
        else
        {
            // 途中段階または初期状態のテキストを表示
            DisplayConversationPage($"{initialConversationId}_{currentStage}");
        }
    }
}