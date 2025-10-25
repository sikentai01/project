using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

/// <summary>
/// ボタンの順序クリックを制御するギミック (UI非依存)
/// Stage 0: 未開始/リセット
/// Stage 1: 起動済み (ボタン入力待ち)
/// Stage > CorrectSequence.Count: 完了済み
/// </summary>
public class ButtonSequenceGimmick : GimmickBase
{
    [Header("初期/リセット時に表示するメッセージ")]
    public string initialMessage = "正しい順序でボタンを押してください。";

    [Header("正解のボタン順序 (インデックス 0～3)")]
    [Tooltip("クリックすべきオブジェクトのインデックスを順番に設定")]
    public List<int> correctSequence = new List<int> { 0, 1, 2, 3 };

    // ★★★ 完了時のオブジェクトアクション ★★★
    [Header("完了時のオブジェクトアクション")]
    [Tooltip("即座に表示するオブジェクト")]
    public GameObject objectToShow;
    [Tooltip("0.5秒後に非表示にするオブジェクト")]
    public GameObject objectToHideAfterDelay;

    // ★★★ リトライ/フィードバック用のフィールドを追加 ★★★
    [Header("フィードバック設定")]
    public string correctFeedbackId = "SEQUENCE_SUCCESS";
    public string incorrectFeedbackId = "SEQUENCE_FAILURE";

    [Header("リトライ設定")]
    public int maxRetriesBeforeReset = 3; // 不正解時にリセットされるまでの許容回数
    private int retryCount = 0; // 現在の不正解回数
    // ★★★ ここまで ★★★


    [Header("クリック可能なオブジェクト群")]
    public SequenceClickableObject[] clickableObjects = new SequenceClickableObject[4];

    private bool isPlayerNear = false;
    private List<int> inputSequence = new List<int>();


    void Awake()
    {
        // 依存削除済み
    }

    void Start()
    {
        // ロードや他の処理が完了するのを待ってから初期化を実行
        StartCoroutine(DelayedInitialization());

        // ★★★ 追加: セーブデータがない場合のチェックをStartから起動 ★★★
        if (gimmickID != "")
        {
            StartCoroutine(CheckForResetOnStart());
        }
    }

    // =====================================================
    // ★★★ 遅延初期化とオブジェクト検索 ★★★
    // =====================================================
    private IEnumerator DelayedInitialization()
    {
        yield return null; // 1フレーム待機し、他のStart()処理の完了を待つ

        InitializeClickableObjects();

        // 進行度に応じてオブジェクトの状態を更新
        UpdateObjectVisibility(currentStage > 0 && currentStage < correctSequence.Count + 1);

        // 完了状態を反映 (ロード後に currentStage が復元されている)
        ApplyCompletionState(currentStage >= correctSequence.Count + 1);
    }

    /// <summary>
    /// クリック可能なオブジェクトへの参照を確立
    /// </summary>
    private void InitializeClickableObjects()
    {
        // clickableObjects配列を使ってSequenceClickableObjectにギミック本体の参照を設定
        for (int i = 0; i < clickableObjects.Length; i++)
        {
            if (clickableObjects[i] != null)
            {
                clickableObjects[i].targetGimmick = this;
            }
        }
    }

    // =====================================================
    // ★★★ セーブデータがない場合の強制リセットロジック ★★★
    // =====================================================
    private IEnumerator CheckForResetOnStart()
    {
        yield return null;

        var data = SaveSystem.LoadGame(1);

        if (data == null)
        {
            if (this.currentStage != 0)
            {
                this.currentStage = 0;
                inputSequence.Clear();

                UpdateObjectVisibility(false);
                ApplyCompletionState(false);

                Debug.Log($"[Sequence] 【強制リセット】セーブデータなし。Stage 0 に戻しました。");
            }
        }
    }


    // =====================================================
    // ★★★ トリガー機能の統合 (GimmickTriggerの代替) ★★★
    // =====================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;

            // 既に起動済みの場合、オブジェクトをすぐに表示（Enterキー不要）
            if (currentStage >= 1)
            {
                UpdateObjectVisibility(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;

            // 範囲外に出たらオブジェクトを非表示に戻す
            UpdateObjectVisibility(false);
        }
    }

    void Update()
    {
        if (currentStage >= correctSequence.Count + 1) return; // 既に完了済みは無視

        // Enterキー入力によるギミック起動
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            StartSequence();
        }
    }

    // =====================================================
    // ギミック起動とボタン操作
    // =====================================================

    public void StartSequence()
    {
        if (currentStage >= correctSequence.Count + 1)
        {
            DisplayMessage("ギミックは既に解除されています。");
            return;
        }

        // --- 初回起動/リセット時の処理 ---
        if (currentStage < 1)
        {
            this.currentStage = 1;
            inputSequence.Clear();
            DisplayMessage(initialMessage);
        }
        else
        {
            // 既に起動状態の場合、現在のステップのヒントを再表示
            DisplayMessage($"ステップ {inputSequence.Count + 1} を入力してください。");
        }

        UpdateObjectVisibility(true); // オブジェクトを表示
    }

    /// <summary>
    /// SequenceClickableObjectがクリックで呼び出すメソッド
    /// </summary>
    public void OnButtonClick(int clickedIndex)
    {
        if (!IsSequenceActive()) return;

        int currentStep = inputSequence.Count;

        if (currentStep < correctSequence.Count)
        {
            int expectedIndex = correctSequence[currentStep];

            if (clickedIndex == expectedIndex)
            {
                // --- 正解処理 ---
                retryCount = 0; // リトライカウントをリセット
                inputSequence.Add(clickedIndex);
                this.currentStage++;

                Debug.Log($"[Sequence] 正解！ステップ {inputSequence.Count}/{correctSequence.Count}");

                if (this.currentStage >= correctSequence.Count + 1)
                {
                    // 最終ステップで完全正解
                    DisplayMessage(correctFeedbackId);
                    CompleteGimmick();
                    return;
                }

                // 次のテキストを表示 (ステップ継続)
                DisplayMessage($"正解！ステップ {inputSequence.Count + 1} を入力してください。");
            }
            else
            {
                // --- 不正解処理 (リトライ判定) ---
                retryCount++;

                if (retryCount >= maxRetriesBeforeReset)
                {
                    // リトライ回数を超過したため、シーケンスをリセット
                    inputSequence.Clear();
                    this.currentStage = 1;
                    retryCount = 0;
                    DisplayMessage(incorrectFeedbackId); // 不正解テキスト表示
                }
                else
                {
                    // リトライ回数内のため、ギミックを継続（シーケンスの状態は維持）
                    DisplayMessage($"不正解。リトライ残り {maxRetriesBeforeReset - retryCount} 回。");
                }
            }
        }
    }

    private void CompleteGimmick()
    {
        Debug.Log("ギミック解除成功！");

        // 1. クリックオブジェクトを非表示
        UpdateObjectVisibility(false);

        // 2. 遅延処理の開始
        StartCoroutine(ExecuteDelayedActions());

        // 3. 進行度を最終段階に設定（セーブ用）
        this.currentStage = correctSequence.Count + 1;
    }

    // =====================================================
    // ★★★ 遅延処理用コルーチン ★★★
    // =====================================================

    private IEnumerator ExecuteDelayedActions()
    {
        // 1. objectToShowを即座に表示
        ApplyCompletionState(true);

        // 2. 0.5秒待機
        yield return new WaitForSeconds(0.5f);

        // 3. objectToHideAfterDelayを非表示にする
        if (objectToHideAfterDelay != null)
        {
            objectToHideAfterDelay.SetActive(false);
            Debug.Log("[Sequence] 0.5秒後、遅延対象のオブジェクトを非表示にしました。");
        }
    }

    /// <summary>
    /// 完了状態に基づき、新規追加されたオブジェクトの状態を更新
    /// </summary>
    private void ApplyCompletionState(bool isCompleted)
    {
        if (objectToShow != null)
        {
            // objectToShowは完了時に表示
            objectToShow.SetActive(isCompleted);
        }

        // objectToHideAfterDelayは、完了直後（isCompleted=true）はそのまま
        // 初期状態(isCompleted=false)は表示にしておく
        if (objectToHideAfterDelay != null)
        {
            objectToHideAfterDelay.SetActive(!isCompleted);
        }
    }


    // =====================================================
    // ヘルパーメソッド
    // =====================================================

    /// <summary>
    /// クリック可能なオブジェクトの表示/非表示を制御する
    /// </summary>
    private void UpdateObjectVisibility(bool isVisible)
    {
        if (clickableObjects != null)
        {
            foreach (var obj in clickableObjects)
            {
                if (obj != null)
                {
                    obj.gameObject.SetActive(isVisible);
                }
            }
        }
    }

    /// <summary>
    /// シーケンスが現在入力可能状態か
    /// </summary>
    public bool IsSequenceActive()
    {
        return currentStage >= 1 && currentStage < correctSequence.Count + 1;
    }

    /// <summary>
    /// 中央パネルにテキストを表示するヘルパーメソッド (Debug.Logに置き換え)
    /// </summary>
    public void DisplayMessage(string message)
    {
        Debug.Log($"[Sequence Message] {message}");
    }


    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);

        inputSequence.Clear();

        if (currentStage > 0 && currentStage <= correctSequence.Count)
        {
            inputSequence = Enumerable.Repeat(0, currentStage - 1).ToList();
        }

        bool completed = currentStage >= correctSequence.Count + 1;

        // ロード後に状態を反映
        UpdateObjectVisibility(!completed);
        ApplyCompletionState(completed);
        InitializeClickableObjects();
    }
}