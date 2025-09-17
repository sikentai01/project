using UnityEngine;

public class LevelSpawnRouter2D : MonoBehaviour
{
    // ★シーン間で持ち回る行き先名（DoorController から代入）
    public static string NextSpawnPointName = "Spawn_Default";

    [Header("未設定や不正時のフォールバック名")]
    [SerializeField] private string fallbackSpawnName = "Spawn_Default";

    [Header("消し忘れ防止（使い終わったら初期化）")]
    [SerializeField] private bool clearAfterUse = true;

    void Start()
    {
        // プレイヤー取得
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        // 使うスポーン名を決定
        string spawnName = string.IsNullOrEmpty(NextSpawnPointName) ? fallbackSpawnName : NextSpawnPointName;

        // シーン内の Spawn を名前で検索
        var spawn = GameObject.Find(spawnName);
        if (spawn == null)
        {
            Debug.LogWarning($"[LevelSpawnRouter2D] Spawn '{spawnName}' が見つかりません。配置をスキップします。");
            return;
        }

        // 2D移動：Rigidbody2Dがあればそちらを優先
        var rb2d = player.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.linearVelocity = Vector2.zero;
            rb2d.position = (Vector2)spawn.transform.position;
        }
        else
        {
            var p = player.transform.position;
            p.x = spawn.transform.position.x;
            p.y = spawn.transform.position.y;
            player.transform.position = p;
        }

        // 次回の暴発防止
        if (clearAfterUse) NextSpawnPointName = fallbackSpawnName;
    }
}
