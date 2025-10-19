using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FallTrap : MonoBehaviour
{
    private Collider2D trapCollider;

    void Start()
    {
        trapCollider = GetComponent<Collider2D>();

        // すでにフラグが立っている（例: セーブ後）なら罠を無効化
        if (GameFlags.Instance != null && GameFlags.Instance.HasFlag("SaveTriggered"))
        {
            DisableTrap();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // トリガーでない場合は無視
        if (!trapCollider.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("[FallTrap] プレイヤーが落とし穴に落下 → GameOverへ");

            // プレイヤーの操作を無効化
            var move = other.GetComponent<GridMovement>();
            if (move != null)
            {
                move.enabled = false;
                Debug.Log("[FallTrap] プレイヤー操作を停止しました");
            }

            // 少し遅らせてゲームオーバー画面を表示
            StartCoroutine(ShowGameOverRoutine());
        }
    }

    private IEnumerator ShowGameOverRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        // BootLoaderを取得
        var boot = FindObjectOfType<BootLoader>();
        if (boot == null)
        {
            Debug.LogWarning("[FallTrap] BootLoaderが見つかりません。GameOverを直接有効化します。");
        }

        // Sceneを直接操作（Additive構成に対応）
        Scene gameOverScene = SceneManager.GetSceneByName("GameOver");
        if (gameOverScene.IsValid() && gameOverScene.isLoaded)
        {
            // GameOverシーンをON
            foreach (var root in gameOverScene.GetRootGameObjects())
                root.SetActive(true);

            Debug.Log("[FallTrap] 既存のGameOverシーンを再有効化しました");
        }
        else
        {
            // 未ロードならロード
            Debug.Log("[FallTrap] GameOverシーンを新規ロードします");
            yield return SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);
        }

        // ゲームシーン（Scene系）を非アクティブに
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name.StartsWith("Scene"))
            {
                foreach (var root in s.GetRootGameObjects())
                    root.SetActive(false);
            }
        }

        // 確実にGameOverがアクティブになるよう設定
        Scene goScene = SceneManager.GetSceneByName("GameOver");
        if (goScene.IsValid())
            SceneManager.SetActiveScene(goScene);

        Debug.Log("[FallTrap] ゲームシーン非表示 → GameOver表示完了");
    }

    // ====== 落とし穴を「無効化」 ======
    public void DisableTrap()
    {
        if (trapCollider != null)
        {
            trapCollider.isTrigger = false; // 壁扱いにする
            Debug.Log("[FallTrap] 落とし穴を無効化 → 壁にしました");
        }
    }
}