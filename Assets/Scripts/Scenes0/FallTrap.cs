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

            //  RoomLoaderを使ってGameOverをAdditiveでロード
            RoomLoader.LoadRoom("GameOver", null);

            // 現在のシーンをアンロード（Additive構成の整理）
            StartCoroutine(UnloadCurrentScene());
        }
    }

    private System.Collections.IEnumerator UnloadCurrentScene()
    {
        yield return new WaitForSeconds(0.5f);

        Scene current = SceneManager.GetActiveScene();
        if (current.IsValid() && current.isLoaded && current.name.StartsWith("Scene"))
        {
            SceneManager.UnloadSceneAsync(current);
            Debug.Log($"{current.name} を破棄しました");
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
