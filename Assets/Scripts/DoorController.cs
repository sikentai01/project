using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    [Header("行き先（TargetScene が空なら同一シーン移動）")]
    [SerializeField] private string targetScene = "";     // 例: "Room_B2"
    [SerializeField] private string targetSpawn = "SpawnPoint";

    [Header("操作キー")]
    [SerializeField] private KeyCode openKey = KeyCode.E;

    [Header("条件（任意）")]
    [SerializeField] private bool requireFlag = false;
    [SerializeField] private BoolReference flagRef;

    [Header("演出（任意）")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private AudioSource doorSE;

    bool isPlayerInside;
    bool isOpening;
    Transform player;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInside = true;
        player = other.transform;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInside = false;
        player = null;
    }

    void Update()
    {
        if (!isPlayerInside || isOpening) return;
        if (Input.GetKeyDown(openKey))
        {
            if (requireFlag && (flagRef == null || !flagRef.Value))
            {
                Debug.Log("[Door] 条件未達 - 扉は開かない");
                return;
            }
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        isOpening = true;
        if (doorAnimator) doorAnimator.SetTrigger("Open");
        if (doorSE) doorSE.Play();

        // ───────────────────────────────
        // ① シーンまたぎ（SpawnRouter経由）
        // ───────────────────────────────
        if (!string.IsNullOrEmpty(targetScene))
        {
            LevelSpawnRouter2D.NextSpawnPointName = string.IsNullOrEmpty(targetSpawn) ? "SpawnPoint" : targetSpawn;
            LevelSpawnRouter2D.PendingSceneName   = targetScene;
            LevelSpawnRouter2D.HasPendingTeleport = true;

            Debug.Log($"[Door] 🚪 {gameObject.name} → {targetScene} / Spawn='{targetSpawn}'");

            // Additiveロード
            // Additiveロード
var async = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
if (async == null)
{
    Debug.LogError($"[Door] ❌ Scene '{targetScene}' のロードに失敗しました（Build Settings未登録の可能性）");
}
else
{
    Debug.Log($"[Door] ✅ Additiveロード開始: {targetScene}");
    async.completed += _ => Debug.Log($"[Door] 🎯 Additiveロード完了: {targetScene}");
}

        }

        // ───────────────────────────────
        // ② 同一シーン内のテレポート
        // ───────────────────────────────
        var spawn = GameObject.Find(string.IsNullOrEmpty(targetSpawn) ? "SpawnPoint" : targetSpawn);
        if (!player)
        {
            Debug.LogWarning("[Door] player が見つかりません");
        }
        else if (!spawn)
        {
            Debug.LogWarning($"[Door] Spawn '{targetSpawn}' が見つからないため扉位置に仮配置");
            player.position = transform.position;
        }
        else
        {
            var rb = player.GetComponent<Rigidbody2D>();
            if (rb)
            {
#if UNITY_6000_0_OR_NEWER
                rb.linearVelocity = Vector2.zero;
#else
                rb.velocity = Vector2.zero;
#endif
                rb.simulated = false;
                rb.position  = (Vector2)spawn.transform.position;
                rb.simulated = true;
                Physics2D.SyncTransforms();
            }
            else
            {
                player.position = spawn.transform.position;
                Physics2D.SyncTransforms();
            }
        }

        isOpening = false;
    }
}

[System.Serializable]
public class BoolReference { public bool Value; }
