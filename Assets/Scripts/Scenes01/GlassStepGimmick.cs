using UnityEngine;

public class GlassStepGimmick : GimmickBase
{
    [Header("ギミックID (セーブ用)")]
    public string gimmickID;

    [Header("ワープ/フラグ発動に必要な回数")]
    public int requiredSteps = 5;

    [Header("ワープさせるターゲットオブジェクト (WhiteBloodCell)")]
    public GameObject targetObjectToActivate; // ワープ対象

    [Header("発動に必要なGameFlagsのID (例: PoisonTrigger)")]
    public string requiredFlagID = "PoisonTrigger";

    [Header("起動時に立てるフラグID")]
    public string activeFlagID = "FLAG_WBC_ACTIVE"; // ワープ開始を示すフラグ

    // プレイヤーへの参照はワープに必要なので、静的に取得します
    private Transform playerTransform;

    // ワープが可能な状態（フラグが立ち、回数が満たされた）かどうか
    private bool isWarpReady = false;

    private void Start()
    {
        // プレイヤーオブジェクトのTransformを静的に取得
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // ギミックが完了していない場合は初期状態（非表示）を保証
        if (targetObjectToActivate != null)
        {
            targetObjectToActivate.SetActive(false);
        }

        // ロード時または初期状態で既に条件を満たしているかチェック
        if (currentStage >= requiredSteps)
        {
            // 条件を満たしているが、ワープ自体は踏むたびに実行されるべきなので、ここでは実行しない。
            // 状態だけを 'Ready' にする。
            isWarpReady = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnGlassStepped();
        }
    }

    /// <summary>
    /// ガラスを踏んだときに呼び出すメソッド
    /// </summary>
    public void OnGlassStepped()
    {
        // 1. 前提フラグが立っているかチェック
        bool hasRequiredFlag = GameFlags.Instance != null && GameFlags.Instance.HasFlag(requiredFlagID);

        // 2. 必要なフラグが立っていない場合は、処理を終了
        if (!hasRequiredFlag)
        {
            Debug.Log($"[GlassStepGimmick] 前提フラグ '{requiredFlagID}' が立っていないため、ワープ処理をスキップします。");
            return;
        }

        // 3. 進行度（踏破回数）を増やす
        this.currentStage++;
        Debug.Log($"[GlassStepGimmick] ガラスを踏んだ回数: {currentStage} / {requiredSteps}");

        // 4. ワープの準備完了をチェック
        if (this.currentStage >= requiredSteps)
        {
            isWarpReady = true;
            TryWarpToPlayer();
        }
    }

    private void TryWarpToPlayer()
    {
        // ワープ準備完了、ターゲット、プレイヤーが全て揃っているかチェック
        if (!isWarpReady || targetObjectToActivate == null || playerTransform == null)
        {
            return;
        }

        // A. WhiteBloodCellをアクティブにする
        if (!targetObjectToActivate.activeSelf)
        {
            targetObjectToActivate.SetActive(true);
            Debug.Log($"[GlassStepGimmick] {targetObjectToActivate.name} を起動しました。");

            // B. 初めて起動したときにフラグを立てる
            if (!string.IsNullOrEmpty(activeFlagID))
            {
                GameFlags.Instance?.SetFlag(activeFlagID);
                Debug.Log($"[GlassStepGimmick] 達成フラグ '{activeFlagID}' を立てました。");
            }
        }

        // C. WhiteBloodCellをプレイヤーの位置にワープさせる
        targetObjectToActivate.transform.position = playerTransform.position;
        Debug.Log($"[GlassStepGimmick] {targetObjectToActivate.name} をプレイヤーの位置にワープさせました。");

        // ワープ後の移動がスムーズになるよう、物理演算を同期させる（任意）
        Physics2D.SyncTransforms();
    }

    // =====================================================
    // セーブ・ロードの処理
    // =====================================================
    public override void LoadProgress(int stage)
    {
        currentStage = stage;

        if (currentStage >= requiredSteps)
        {
            // ロード時はワープ準備完了状態にする
            isWarpReady = true;

            // WhiteBloodCellの状態を再現（アクティブにする）
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(true);
            }

            // 達成フラグを復元
            if (!string.IsNullOrEmpty(activeFlagID))
            {
                GameFlags.Instance?.SetFlag(activeFlagID);
            }
        }
        else
        {
            // 未完了時は初期状態に戻す
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(false);
            }
        }
    }
}