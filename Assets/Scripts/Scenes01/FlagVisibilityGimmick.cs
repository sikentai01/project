using UnityEngine;

// GimmickBaseを継承し、特定のGameFlagと連動して表示を制御する
public class FlagVisibilityGimmick : GimmickBase
{
    [Header("表示を切り替えるターゲット")]
    public GameObject targetObject;

    [Header("表示に必要なGameFlagsのID")]
    public string requiredFlagID;

    [Header("フラグが立ったら表示する (true) or 非表示にする (false)")]
    public bool activateOnFlag = true;

    private void Start()
    {
        if (string.IsNullOrEmpty(requiredFlagID))
        {
            Debug.LogError($"[FlagVisibilityGimmick] {gameObject.name}: Required Flag IDが設定されていません。");
            return;
        }

        // ギミックの初期状態とロード状態の確認
        CheckFlagAndApplyVisibility();
    }

    private void Update()
    {
        // 動作中のフラグチェックは負荷になる可能性があるため、
        // 頻繁な切り替えを想定しない場合はUpdateは不要。
        // ただし、即時反映が必要な場合はこのチェックを残す。
        CheckFlagAndApplyVisibility();
    }

    /// <summary>
    /// GameFlagsの状態を確認し、表示状態を適用する
    /// </summary>
    // FlagVisibilityGimmick.cs
    public void CheckFlagAndApplyVisibility()
    {
        if (GameFlags.Instance == null) return;
        if (targetObject == null)
        {
            Debug.LogError($"[FlagVis] TargetObjectがNULLです: {gameObject.name}"); // ★ 確認用ログ1
            return;
        }

        bool hasFlag = GameFlags.Instance.HasFlag(requiredFlagID);
        bool shouldBeVisible = (hasFlag && activateOnFlag) || (!hasFlag && !activateOnFlag);

         // ★ 確認用ログ2

        // 現在の表示状態と目標状態が異なるときのみ処理を実行
        if (targetObject.activeSelf != shouldBeVisible)
        {
            targetObject.SetActive(shouldBeVisible);

            this.currentStage = shouldBeVisible ? 1 : 0;

            // ★ 成功ログ
        }
    }

    // GimmickBaseのロード処理をオーバーライドし、状態を再チェック
    public override void LoadProgress(int stage)
    {
        // ロード時にcurrentStageは復元されるが、表示状態はフラグに依存するため再チェック
        base.LoadProgress(stage);
        CheckFlagAndApplyVisibility();
    }
}