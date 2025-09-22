using UnityEngine;
using UnityEngine.SceneManagement;

public class FallTrap : MonoBehaviour
{
    private Collider2D trapCollider;

    void Start()
    {
        trapCollider = GetComponent<Collider2D>();

        // もしセーブ済みなら、最初からトリガーOFFにして壁化
        if (GameFlags.SaveTriggered)
        {
            DisableTrap();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // トリガーじゃない時は何もしない
        if (!trapCollider.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤーが落とし穴に落ちた！");
            // ここでゲームオーバー処理
            SceneManager.LoadScene("GameOver");
        }
    }

    // ====== ここで落とし穴を「無効化」 ======
    public void DisableTrap()
    {
        if (trapCollider != null)
        {
            trapCollider.isTrigger = false; // ← これで壁にする
            Debug.Log("落とし穴を無効化 → 壁にしました");
        }
    }
}
//うまくいってる？