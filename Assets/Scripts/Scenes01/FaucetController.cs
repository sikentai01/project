using UnityEngine;

public class FaucetController : MonoBehaviour
{
    // ... (既存のフィールドは省略) ...
    [Header("水滴のSE")]
    public AudioClip waterDropClip;

    private Animator targetAnimator;

    private void Start()
    {
        // 自身のAnimatorコンポーネントを取得
        targetAnimator = GetComponent<Animator>();

        // ★★★ シーンロード時の誤検知対策 ★★★
        if (targetAnimator != null)
        {
            // アニメーターを無効化し、シーン開始時の自動再生を停止
            targetAnimator.enabled = false;
            Debug.Log("[FaucetController] 初期化完了: アニメーションを停止状態に設定しました。");
        }
    }

    // Faucet.anim のアニメーションイベントから呼び出される（はず）
    public void PlayWaterDropSE()
    {
        if (SoundManager.Instance != null && waterDropClip != null)
        {
            SoundManager.Instance.PlaySE(waterDropClip);
        }
    }

    // プレイヤーが範囲から出た時に呼ばれる
    public void StopFaucetAnimation()
    {
        if (targetAnimator != null)
        {
            targetAnimator.enabled = false;
        }
    }

    // プレイヤーが範囲に入った時に呼ばれる
    public void StartFaucetAnimation()
    {
        if (targetAnimator != null)
        {
            // アニメーションを有効化し、再生を再開
            targetAnimator.enabled = true;
            // アニメーターが既に有効な場合、この行は不要な場合が多いが、念のためPlay(初期ステート)を呼んでも良い
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーが入ったらアニメーションを再開
            StartFaucetAnimation();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーが出たらアニメーションを停止
            StopFaucetAnimation();
        }
    }
}