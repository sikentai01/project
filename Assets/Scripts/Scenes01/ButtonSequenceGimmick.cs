using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

/// <summary>
/// ボタンの順序クリックとテキスト表示を制御するギミック
/// </summary>
public class ButtonSequenceGimmick : GimmickBase
{
    [Header("初期/リセット時に表示するメッセージ")]
    public string initialMessage = "正しい順序でボタンを押してください。";

    [Header("正解のボタン順序 (インデックス 0～3)")]
    [Tooltip("クリックすべきボタンのインデックスを順番に設定")]
    public List<int> correctSequence = new List<int> { 0, 1, 2, 3 };

    // UI連携フィールド
    [Header("UI関連")]
    public GameObject gimmickCanvasRoot;

    private bool isPlayerNear = false;
    private GimmickCanvasController canvasController;

    // currentStage はギミックの進行度
    private List<int> inputSequence = new List<int>();

    void Awake()
    {
        canvasController = FindObjectOfType<GimmickCanvasController>();
    }

    void Start()
    {
        // currentStageが0の場合、インプットシーケンスをクリア
        if (currentStage <= correctSequence.Count)
        {
            inputSequence.Clear();
        }

        // 起動済み (currentStage > 0) の場合、Canvasを非表示にしておく
        if (currentStage > 0 && canvasController != null)
        {
            HideGimmickCanvas();
        }
    }

    // =====================================================
    // ★★★ ギミック起動 (GimmickBaseのメソッドではない) ★★★
    // =====================================================

    /// <summary>
    /// Enterキー操作などで、ギミックを起動する (overrideを削除)
    /// </summary>
    public void StartSequence() // ★ StartGimmickから名称変更
    {
        if (currentStage >= correctSequence.Count + 1)
        {
            DisplayMessage("ギミックは既に解除されています。");
            return;
        }

        // --- 初回起動/リセット時の処理 ---
        if (currentStage < 1)
        {
            this.currentStage = 1; // 起動状態にする (Stage 1)
            inputSequence.Clear();
            DisplayMessage(initialMessage);
        }
        else
        {
            // 既に起動状態の場合、現在のステップのヒントを再表示
            DisplayMessage($"ステップ {inputSequence.Count + 1} を入力してください。");
        }

        ShowGimmickCanvas();
    }

    // ... (OnTriggerEnter/Exit2D, Update, OnButtonClick は省略) ...

    // GimmickCanvasControllerがボタンクリックで呼び出すメソッド
    public void OnButtonClick(int clickedIndex)
    {
        if (currentStage < 1) return;
        if (currentStage >= correctSequence.Count + 1) return;

        int currentStep = inputSequence.Count;

        if (currentStep < correctSequence.Count)
        {
            int expectedIndex = correctSequence[currentStep];

            if (clickedIndex == expectedIndex)
            {
                // 正解処理
                inputSequence.Add(clickedIndex);
                this.currentStage++;

                if (this.currentStage >= correctSequence.Count + 1)
                {
                    CompleteGimmick();
                    return;
                }

                // 次のテキストを表示
                Debug.Log($"[Sequence] 正解。次のステップへ ({inputSequence.Count}/{correctSequence.Count})");
                DisplayMessage($"正解！ステップ {inputSequence.Count + 1} を入力してください。");
            }
            else
            {
                // 不正解処理
                inputSequence.Clear();
                this.currentStage = 1;
                Debug.Log($"[Sequence] 不正解。シーケンスをリセットしました。");
                DisplayMessage("不正解です。最初からやり直してください。");
            }
        }
    }


    private void CompleteGimmick()
    {
        DisplayMessage("ギミック解除成功！");

        if (canvasController != null)
        {
            canvasController.HideCanvas();
        }

        this.currentStage = correctSequence.Count + 1;
        Debug.Log($"[Sequence] ギミック解除完了！");
    }

    // ... (Show/HideGimmickCanvas, DisplayMessage は省略) ...
    public void DisplayMessage(string message)
    {
        if (canvasController != null)
        {
            canvasController.SetCenterMessage(message);
        }
    }

    public void ShowGimmickCanvas()
    {
        if (canvasController != null) canvasController.gameObject.SetActive(true);
    }

    public void HideGimmickCanvas()
    {
        if (canvasController != null) canvasController.gameObject.SetActive(false);
    }

    // =====================================================
    // GimmickBaseのメソッド
    // =====================================================

    // GimmickBaseには StartGimmick はないので、抽象クラスGimmickBaseのメソッドは省略します。
    // StartGimmickが必要な場合は、以下のダミーメソッドを作成してください。

    // public override void StartGimmick(ItemTrigger trigger) { /* 処理なし */ }


    // ★★★ GimmickTriggerの役割を代替するメソッドはそのまま維持 ★★★
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (currentStage >= 1 && canvasController != null) ShowGimmickCanvas();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            HideGimmickCanvas();
        }
    }

    void Update()
    {
        if (currentStage >= correctSequence.Count + 1) return;
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            StartSequence(); // 修正後のメソッドを呼び出し
        }
    }

    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        // ロード後の復元処理
        inputSequence.Clear();

        // currentStageが1以上の場合、インプットシーケンスを復元
        if (currentStage > 0 && currentStage <= correctSequence.Count)
        {
            inputSequence = Enumerable.Repeat(0, currentStage - 1).ToList();
        }
    }
}