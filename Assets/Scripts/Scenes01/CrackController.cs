using UnityEngine;

// Unityの階層構造を参考に、GimmickBaseと連携するController
public class CrackController : MonoBehaviour
{
    [Header("対象のギミックID (VisibilityGimmickに設定したものと一致させる)")]
    public string targetGimmickID;

    [Header("表示/非表示を切り替えるギミック本体")]
    public VisibilityGimmick targetGimmick;

    private void Start()
    {
        // === ギミック本体の検索処理 ===
        if (targetGimmick == null)
        {
            var gimmicks = Object.FindObjectsByType<VisibilityGimmick>(FindObjectsSortMode.None);
            foreach (var g in gimmicks)
            {
                if (g.gimmickID == targetGimmickID)
                {
                    targetGimmick = g;
                    break;
                }
            }
        }

        if (targetGimmick == null)
        {
            Debug.LogError($"[CrackController] ID:{targetGimmickID} に対応する VisibilityGimmick が見つかりません！");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetGimmick != null)
        {
            // 既に表示済み（currentStageが1以上）なら何もしない
            if (targetGimmick.currentStage >= 1)
                return;

            // 未完了の状態なので、表示を実行 (stage=1)
            targetGimmick.SetVisibility(1);

            Debug.Log($"[CrackController] プレイヤーが侵入 → Inside-Town-E_12を表示");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 1回表示方式では、一度表示したら非表示に戻さないため、特に処理は行いません。
    }
}