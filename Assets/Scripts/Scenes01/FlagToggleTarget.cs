using UnityEngine;

public class FlagToggleTarget : MonoBehaviour
{
    [Header("フラグの状態を監視するID")]
    public string targetFlagID = "FLAG_WBC_ACTIVE";

    [Header("切り替え対象のオブジェクト")]
    public GameObject targetObject;

    [Header("フラグが立ったら True にする（True:表示 / False:非表示）")]
    public bool stateOnFlag = true;

    private bool previousFlagState = false;

    private void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError($"[FlagToggleTarget] {gameObject.name}: Target Objectが設定されていません。");
            this.enabled = false;
            return;
        }

        // 初回チェックと初期状態の適用
        CheckFlagAndApplyState();
    }

    private void Update()
    {
        // 毎フレームチェックして、フラグ状態の変化に反応する
        CheckFlagAndApplyState();
    }

    private void CheckFlagAndApplyState()
    {
        if (GameFlags.Instance == null) return;

        bool currentFlagState = GameFlags.Instance.HasFlag(targetFlagID);

        // 現在のターゲットのアクティブ状態
        bool currentTargetActive = targetObject.activeSelf;

        // フラグ状態から算出されるべきターゲットの目標状態
        bool desiredState = currentFlagState == stateOnFlag; // フラグが立ったら目標状態になる

        // フラグ状態が変化したか、または現在の状態が目標と異なるときにのみ処理
        if (currentFlagState != previousFlagState || currentTargetActive != desiredState)
        {
            targetObject.SetActive(desiredState);
            previousFlagState = currentFlagState;

            Debug.Log($"[FlagToggleTarget] {targetFlagID} 変化検出。{targetObject.name} を {(desiredState ? "表示" : "非表示")} に切り替えました。");
        }
    }
}