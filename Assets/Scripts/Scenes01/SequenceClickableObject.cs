using UnityEngine;

public class SequenceClickableObject : MonoBehaviour
{
    [Header("このオブジェクトのインデックス")]
    public int sequenceIndex;

    [Header("クリック時に再生するSE (任意)")]
    public AudioClip clickSE;

    // ギミック本体への参照
    public ButtonSequenceGimmick targetGimmick;

    // ★★★ 修正箇所: Awakeで非表示を強制する ★★★
    private void Awake()
    {
        // AwakeはStartより先に実行されるため、Inspectorの設定を上書きし、即座に非表示を保証する
        gameObject.SetActive(false);
    }
    // ★★★ ここまで修正 ★★★

    private void OnMouseDown()
    {
        if (targetGimmick != null && targetGimmick.IsSequenceActive())
        {
            // SE再生処理
            if (SoundManager.Instance != null && clickSE != null)
            {
                SoundManager.Instance.PlaySE(clickSE);
            }

            // ギミック本体にクリックを通知
            targetGimmick.OnButtonClick(sequenceIndex);
            Debug.Log($"[Clickable] Index {sequenceIndex} をクリックしました。");
        }
    }

    // Start() メソッドは不要になったため削除します
}