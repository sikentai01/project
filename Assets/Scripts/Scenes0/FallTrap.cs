using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FallTrap : MonoBehaviour
{
    private Collider2D trapCollider;

    void OnEnable()
    {
        trapCollider = GetComponent<Collider2D>();

        // はじめから時には強制ON（SaveTriggeredが無いならトリガー有効）
        if (GameFlags.Instance == null || !GameFlags.Instance.HasFlag("SaveTriggered"))
        {
            trapCollider.isTrigger = true;
            Debug.Log("[FallTrap] フラグなし → トリガー再有効化");
        }
        else
        {
            trapCollider.isTrigger = false;
            Debug.Log("[FallTrap] セーブ後 → トリガー無効化（壁）");
        }
    }

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

        //  BootLoaderでプレイヤー初期移動中なら発動しない
        if (BootLoader.IsPlayerSpawning)
        {
            Debug.Log("[FallTrap] プレイヤー初期移動中 → 無視");
            return;
        }

        // すでにGameOver遷移中なら重複防止
        if (BootLoader.IsTransitioning)
        {
            Debug.Log("[FallTrap] シーン遷移中のため無視");
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("[FallTrap] プレイヤーが落とし穴に落下 → GameOverへ");

            // プレイヤーの操作を無効化
            var move = other.GetComponent<GridMovement>();
            if (move != null)
            {
                move.ForceStopMovement(); // ★ 確実に停止
                move.enabled = false;
                Debug.Log("[FallTrap] プレイヤー操作を停止しました");
            }

            // 少し遅らせてゲームオーバー画面を表示
            StartCoroutine(ShowGameOverRoutine());
        }
    }

    private IEnumerator ShowGameOverRoutine()
    {
        // 落下演出などのため少し待機
        yield return new WaitForSeconds(0.5f);

        Debug.Log("[FallTrap] GameOverシーンへ切替要求");
        BootLoader.Instance.SwitchSceneInstant("GameOver"); // ★ BootLoaderの即切替メソッドを使用
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
