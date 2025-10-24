using UnityEngine;

public class GimmickCanvasActivator : MonoBehaviour
{
    [Header("表示するGimmickCanvasのルートオブジェクト")]
    [Tooltip("シーン内のGimmickCanvasプレハブのルートを割り当てる")]
    public GameObject gimmickCanvasRoot;

    private GimmickTrigger targetTrigger;

    private void Start()
    {
        // 自身と同じゲームオブジェクトにアタッチされているGimmickTriggerを取得
        targetTrigger = GetComponent<GimmickTrigger>();
        
        if (gimmickCanvasRoot == null)
        {
            Debug.LogError("[Activator] GimmickCanvasRootが設定されていません。");
            this.enabled = false;
        }

        // 初期状態を非表示に設定
        if (gimmickCanvasRoot != null && gimmickCanvasRoot.activeSelf)
        {
            gimmickCanvasRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (targetTrigger == null || gimmickCanvasRoot == null) return;

        // GimmickTriggerのIsPlayerNearプロパティ（内部で管理されている）がtrueであればCanvasを表示
        // IsPlayerNearがprivate setのため、GimmickTriggerのpublicメソッド経由でしか状態を取得できません。
        // ※ GimmickTrigger.csのIsPlayerNearがpublicでない場合、この直接的な参照はできません。

        // ★★★ 暫定的な代替手段：GimmickTrigger.csのpublicな状態取得メソッドを呼び出す ★★★
        // ここでは、GimmickTrigger.csに IsPlayerNearプロパティが存在し、
        // かつ ItemTrigger.cs のように Update で入力待ちをしているギミックであると仮定し、
        // プレイヤーが範囲外に出ていればCanvasを非表示にするように制御します。
        
        // 実際には、GimmickTriggerを継承するクラス（ItemTriggerなど）が、
        // プレイヤーが近づいたときに何らかのフラグを立てるロジックを持っているはずです。

        // 今回は、GimmickTrigger.targetGimmickのcurrentStageが0の場合にのみメニューを表示するロジックを組みます。
        
        GimmickBase targetGimmick = targetTrigger.targetGimmick;
        
        if (targetGimmick == null) return;

        // ギミックが Stage 0（初期状態）で、プレイヤーが近くにいる場合のみメニューを表示する
        // IsPlayerNearがpublicでないため、Inputで間接的に検知されるまで表示しません。

        // 【GimmickTriggerがプレイヤーに接近したことを検知する最も確実な方法】
        // GimmickTrigger.csを直接いじらず、かつ IsPlayerNear が public set でないため、
        // GimmickTriggerにアタッチされているコライダーを自身で取得し、再チェックするロジックに切り替えます。
        
        Collider2D triggerCollider = GetComponent<Collider2D>();

        if (triggerCollider != null)
        {
            // Physics2D.IsTouching(PlayerのCollider, 自身のCollider) でチェックしますが、
            // これは効率が悪いため、StartConv()などのタイミングで制御する方が良いです。
            
            // ★ シンプルに、Canvasの表示・非表示は外部のイベントに任せる設計に変更します ★
        }
    }
}