using UnityEngine;

public class FaucetController : MonoBehaviour
{
    // アニメーションイベントからSEを再生する機能はそのまま残します
    [Header("再生する水滴のSE")]
    public AudioClip waterDropClip;

    // このコンポーネントと同じオブジェクトのアニメーター
    private Animator targetAnimator;

    private void Start()
    {
        // 自身のAnimatorコンポーネントを取得
        targetAnimator = GetComponent<Animator>();

        // 範囲外で初期アニメーションが再生されている場合は、ここで止めるなどの処理を入れる
    }

    // Animation Eventから呼び出されるメソッド（SE再生はそのまま）
    public void PlayWaterDropSE()
    {
        if (SoundManager.Instance != null && waterDropClip != null)
        {
            SoundManager.Instance.PlaySE(waterDropClip);
            Debug.Log("[FaucetController] アニメーションイベントにより水滴SEを再生しました。");
        }
        else
        {
            Debug.LogWarning("[FaucetController] SoundManagerまたはwaterDropClipが設定されていません。");
        }
    }

    /// <summary>
    /// プレイヤーが範囲から出たときにアニメーションを停止する
    /// </summary>
    public void StopFaucetAnimation()
    {
        if (targetAnimator != null)
        {
            // Animatorコンポーネントを無効化することでアニメーションを停止させる
            targetAnimator.enabled = false;

            // 💡 補足: アニメーションを再開させるには targetAnimator.enabled = true; を呼ぶ必要があります
            Debug.Log("[FaucetController] プレイヤーが範囲から出たため、蛇口のアニメーションを停止しました。");
        }
    }

    /// <summary>
    /// プレイヤーが範囲に入ったときにアニメーションを再開する
    /// </summary>
    public void StartFaucetAnimation()
    {
        if (targetAnimator != null && !targetAnimator.enabled)
        {
            // Animatorコンポーネントを有効化してアニメーションを再開させる
            targetAnimator.enabled = true;
            Debug.Log("[FaucetController] プレイヤーが範囲に入ったため、蛇口のアニメーションを再開しました。");
        }
    }


    // =====================================================
    // 範囲外検出
    // =====================================================
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