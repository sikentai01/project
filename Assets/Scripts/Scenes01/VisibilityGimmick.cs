using UnityEngine;

// GimmickBaseを継承し、表示・非表示を制御する
public class VisibilityGimmick : GimmickBase
{
    [Header("表示を切り替えるターゲット")]
    public GameObject targetObject;

    /// <summary>
    /// ギミックの状態を更新する
    /// stage: 0=非表示, 1=表示
    /// </summary>
    /// <param name="stage">新しいステージ</param>
    public void SetVisibility(int stage)
    {
        if (targetObject == null)
        {
            Debug.LogWarning($"[VisibilityGimmick] TargetObjectが設定されていません。ID: {gimmickID}");
            return;
        }

        // stage 1なら表示、0なら非表示
        bool isVisible = (stage == 1);
        targetObject.SetActive(isVisible);
        this.currentStage = stage; // GimmickBaseの進行段階を更新

        Debug.Log($"[VisibilityGimmick] {targetObject.name} の表示を {(isVisible ? "ON" : "OFF")} にしました。 (Stage: {stage})");
    }

    // ロード時に状態を復元
    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);
        // ロード時にも表示状態を反映させる
        SetVisibility(stage);
    }

    private void Awake()
    {
        // ゲーム開始時に現在のstageに基づいて初期表示を設定（LoadProgressで上書きされる可能性あり）
        if (targetObject != null)
        {
            SetVisibility(this.currentStage);
        }
    }
}