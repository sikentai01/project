using UnityEngine;
using System.Collections;

public class DoorAnimationController : MonoBehaviour
{
    public Animator characterAnimator;
    public MonoBehaviour movementScript;

    private bool isAnimationPlaying = false;

    // トリガーコライダーに入ったときに一度だけ呼び出される
    void OnTriggerEnter2D(Collider2D other)
    {
         Debug.Log("ドアに近づきました"); // デバッグ用
    }

    // トリガーコライダーの中にいる間、毎フレーム呼び出される
    void OnTriggerStay2D(Collider2D other)
    {
        // アニメーション再生中でない、かつEnterキーが押された場合
        if (!isAnimationPlaying && other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Return))
        {
            isAnimationPlaying = true;

            // 移動スクリプトを一時的に無効化する
            if (movementScript != null)
            {
                movementScript.enabled = false;
            }

            // アニメーションを再生
            characterAnimator.Play("OpenGate");

            // アニメーションが終了するまで待機するコルーチンを開始
            StartCoroutine(WaitForAnimationEnd());
        }
    }

    // トリガーコライダーから出たときに呼び出される
    void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log("ドアから離れました"); // デバッグ用
    }

    private IEnumerator WaitForAnimationEnd()
    {
        yield return null;

        yield return new WaitForSeconds(characterAnimator.GetCurrentAnimatorStateInfo(0).length);

        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
        isAnimationPlaying = false;
    }
}