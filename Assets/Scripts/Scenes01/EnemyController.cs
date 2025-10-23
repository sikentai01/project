using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed = 1.5f;

    [Header("追跡開始までの遅延時間（秒）")]
    public float startDelayTime = 1.5f; // 猶予期間

    private Transform targetPlayer;
    private bool isTracking = false;

    // EnemyController.cs の Start() メソッド（最終修正版）

    private void Start()
    {
        // 1. プレイヤーのTransformを検索
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            targetPlayer = playerObj.transform;
        }
        else
        {
            Debug.LogError("[EnemyController] 'Player'タグを持つオブジェクトが見つかりません。");
            this.enabled = false;
            return;
        }

        // 2. ★★★重要★★★ Start() から自動で追跡を開始するコルーチン呼び出しを削除！
        // StartCoroutine(StartTrackingAfterDelay()); // この行は削除
    }

    /// <summary>
    /// 外部から追跡を開始させる（GlassStepGimmickから呼ばれる）
    /// </summary>
    public void StartTracking()
    {
        if (!isTracking)
        {
            // 猶予期間を設けるためのコルーチンを起動
            StartCoroutine(StartTrackingAfterDelay());
        }
    }

    private IEnumerator StartTrackingAfterDelay()
    {
        isTracking = false;
        // 猶予期間を設定
        yield return new WaitForSeconds(startDelayTime);
        isTracking = true;
        Debug.Log("[EnemyController] 猶予期間終了。追跡を開始します。");
    }

    private void Update()
    {
        if (targetPlayer == null) return;

        // 追跡フラグが true の場合のみ移動を実行
        if (!isTracking) return;

        // プレイヤーに向かって移動
        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}