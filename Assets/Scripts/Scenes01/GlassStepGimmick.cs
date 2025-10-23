using UnityEngine;

public class GlassStepGimmick : GimmickBase
{
    [Header("ギミックID (セーブ用)")]
    public string gimmickID;

    [Header("追跡者を有効化するために必要な回数")]
    public int requiredSteps = 5;

    [Header("表示にするターゲットオブジェクト (WhiteBloodCell)")]
    public GameObject targetObjectToActivate;

    [Header("発動に必要なGameFlagsのID (例: PoisonTrigger)")]
    public string requiredFlagID = "PoisonTrigger";

    [Header("起動時に立てるフラグID")]
    public string activeFlagID = "FLAG_WBC_ACTIVE"; // WhiteBloodCell起動を示すフラグ

    private bool isGimmickCompleted = false;

    private void Start()
    {
        // ... (省略: Start() の初期化処理) ...
        if (currentStage >= requiredSteps)
        {
            TryCompleteGimmick();
        }
        else
        {
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnGlassStepped();
        }
    }

    public void OnGlassStepped()
    {
        if (isGimmickCompleted) return;

        bool hasRequiredFlag = GameFlags.Instance != null && GameFlags.Instance.HasFlag(requiredFlagID);

        if (!hasRequiredFlag)
        {
            Debug.Log($"[GlassStepGimmick] 前提フラグ '{requiredFlagID}' が立っていないため、カウントをスキップします。");
            return;
        }

        this.currentStage++;
        Debug.Log($"[GlassStepGimmick] ガラスを踏んだ回数: {currentStage} / {requiredSteps}");

        TryCompleteGimmick();
    }

    private void TryCompleteGimmick()
    {
        bool enoughSteps = this.currentStage >= requiredSteps;

        if (enoughSteps)
        {
            // A. WhiteBloodCell (追跡者) を表示にする
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(true);
                Debug.Log($"[GlassStepGimmick] {targetObjectToActivate.name} を表示にしました。");

                // ★ 追跡開始のフラグを立てる
                if (!string.IsNullOrEmpty(activeFlagID))
                {
                    GameFlags.Instance?.SetFlag(activeFlagID);
                    Debug.Log($"[GlassStepGimmick] 達成フラグ '{activeFlagID}' を立てました。");
                }

                // ★ 追跡者の移動をONにする！
                var enemyController = targetObjectToActivate.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.StartTracking();
                }
            }

            // --- 完了処理 ---
            isGimmickCompleted = true;
            this.enabled = false;
            Debug.Log("[GlassStepGimmick] 発動条件達成！ギミックを完了しました。");
        }
    }

    public override void LoadProgress(int stage)
    {
        currentStage = stage;

        if (currentStage >= requiredSteps)
        {
            // WhiteBloodCellの状態を再現
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(true);

                // ロード時も追跡を再開させる
                var enemyController = targetObjectToActivate.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.StartTracking();
                }
            }

            // 達成フラグを復元
            if (!string.IsNullOrEmpty(activeFlagID))
            {
                GameFlags.Instance?.SetFlag(activeFlagID);
            }

            isGimmickCompleted = true;
            this.enabled = false;
        }
        else
        {
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(false);
            }
        }
    }
}