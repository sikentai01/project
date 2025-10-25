using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections; // コルーチンのために必要

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
    // [Header("UI関連")] // 以前のフィールドは削除
    // public GameObject gimmickCanvasRoot; 

    // ★★★ 新規追加フィールド ★★★
    [Header("完了時のオブジェクトアクション")]
    [Tooltip("即座に表示するオブジェクト")]
    public GameObject objectToShow;
    [Tooltip("0.5秒後に非表示にするオブジェクト")]
    public GameObject objectToHideAfterDelay;
    // ★★★ ここまで ★★★

    private bool isPlayerNear = false;
    private List<int> inputSequence = new List<int>();

    void Awake()
    {
        // GimmickCanvasControllerへの依存を削除
    }

    void Start()
    {
        InitializeClickableObjects();
        UpdateObjectVisibility(currentStage > 0);

        // 完了状態であれば、新しいオブジェクトの状態も反映
        if (currentStage >= correctSequence.Count + 1)
        {
            ApplyCompletionState(true);
        }
        else
        {
            // 初期状態では objectToShow/objectToHideAfterDelay を非表示/表示に設定
            ApplyCompletionState(false);
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
        if (currentStage >= correctSequence.Count + 1) return;

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
            Debug.Log("ギミックは既に解除されています。");
            return;
        }

        if (currentStage < 1)
        {
            this.currentStage = 1;
            inputSequence.Clear();
            Debug.Log($"[Sequence] START: {initialMessage}");
        }
        else
        {
            Debug.Log($"[Sequence] {inputSequence.Count + 1} ステップ目。");
        }

        UpdateObjectVisibility(true); // オブジェクトを表示
    }

    public void OnButtonClick(int clickedIndex)
    {
        if (!IsSequenceActive()) return;

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
                    CompleteGimmick(); // 報酬なしの完了処理
                    return;
                }

                Debug.Log($"[Sequence] 正解！ステップ {inputSequence.Count}/{correctSequence.Count}");
            }
            else
            {
                // 不正解処理
                inputSequence.Clear();
                this.currentStage = 1;
                Debug.Log($"[Sequence] 不正解。シーケンスをリセットしました。");
            }
        }
    }

    private void CompleteGimmick()
    {
        Debug.Log("ギミック解除成功！");

        // 1. オブジェクトの表示/非表示を即座に切り替える
        UpdateObjectVisibility(false); // クリックオブジェクトを非表示にする

        // 2. ★★★ 遅延処理の開始 ★★★
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

        // objectToHideAfterDelayは、完了直後（isCompleted=true）はそのままにし、
        // コルーチンで非表示にする。初期状態(isCompleted=false)は表示にしておく。
        if (objectToHideAfterDelay != null)
        {
            objectToHideAfterDelay.SetActive(!isCompleted);
        }
    }


    // =====================================================
    // ヘルパーメソッド (InitializeClickableObjects, UpdateObjectVisibility, IsSequenceActive, LoadProgress は省略)
    // =====================================================

    // クリック可能なオブジェクトの配列
    public SequenceClickableObject[] clickableObjects = new SequenceClickableObject[4];

    private void InitializeClickableObjects()
    {
        for (int i = 0; i < clickableObjects.Length; i++)
        {
            if (clickableObjects[i] != null)
            {
                clickableObjects[i].targetGimmick = this;
            }
        }
    }

    private void UpdateObjectVisibility(bool isVisible)
    {
        foreach (var obj in clickableObjects)
        {
            if (obj != null)
            {
                obj.gameObject.SetActive(isVisible);
            }
        }
    }

    public bool IsSequenceActive()
    {
        return currentStage >= 1 && currentStage < correctSequence.Count + 1;
    }

    public override void LoadProgress(int stage)
    {
        // ★★★ GimmickBaseの復元ロジックを呼び出す ★★★
        base.LoadProgress(stage);

        // ロード後の復元処理
        inputSequence.Clear();

        // currentStageが1以上の場合、インプットシーケンスを復元
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