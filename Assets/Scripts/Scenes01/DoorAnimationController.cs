using UnityEngine;
using System.Collections;

public class DoorAnimationController : MonoBehaviour
{
    // ドア自身のAnimatorを割り当てる
    public Animator doorAnimator;

    // プレイヤーの移動スクリプトへの参照
    private MonoBehaviour playerMovementScript;

    private bool playerIsNearDoor = false;
    private bool isAnimationPlaying = false;

    // Startはゲーム開始時に一度だけ呼ばれます
    void Start()
    {
        // シーンに関係なく、GridMovementスクリプトを自動で探して割り当てる
        playerMovementScript = FindObjectOfType<GridMovement>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearDoor = true;
            Debug.Log("プレイヤーがドアに近づいた");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearDoor = false;
            Debug.Log("プレイヤーがドアから離れた");
        }
    }

    void Update()
    {
        if (playerIsNearDoor && !isAnimationPlaying && Input.GetKeyDown(KeyCode.Return))
        {
            isAnimationPlaying = true;

            // プレイヤーの移動スクリプトを一時的に無効化
            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = false;
            }

            // ドアのアニメーションを再生
            doorAnimator.Play("OpenGate", 0);

            // アニメーション終了後に移動を再開させるコルーチンを開始
            StartCoroutine(WaitForAnimationEnd());
        }
    }

    private IEnumerator WaitForAnimationEnd()
    {
        yield return null;

        // 再生中のアニメーションの長さを取得して待機
        yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);

        // プレイヤーの移動スクリプトを有効化
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
        isAnimationPlaying = false;
    }
}