using UnityEngine;

public class SequenceClickableObject : MonoBehaviour
{
    [Header("このオブジェクトのインデックス")]
    public int sequenceIndex;

    [Header("クリック時に再生するSE")] // ★ 新規追加フィールド
    public AudioClip clickSE;

    // ギミック本体への参照
    public ButtonSequenceGimmick targetGimmick;

    private void Start()
    {
        // 初期状態は非表示
        gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (targetGimmick != null && targetGimmick.IsSequenceActive())
        {
            // ★★★ SE再生処理の追加 ★★★
            if (SoundManager.Instance != null && clickSE != null)
            {
                SoundManager.Instance.PlaySE(clickSE);
                Debug.Log($"[Clickable] Index {sequenceIndex} のクリックSEを再生しました。");
            }

            // ギミック本体にクリックを通知
            targetGimmick.OnButtonClick(sequenceIndex);
        }
    }
}