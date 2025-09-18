using UnityEngine;

public class SyncTrigger : MonoBehaviour
{
    // プレイヤーがトリガー内にいるかどうかのフラグ
    public bool isPlayerInside = false;

    // ここに他のSyncTriggerのロジックを追加

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}