using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    [Header("移動先（SpawnPointをドラッグ）")]
    [SerializeField] private Transform targetPoint;

    [Header("操作キー")]
    [SerializeField] private KeyCode openKey = KeyCode.E;

    [Header("ギミック判定（任意）")]
    [SerializeField] private bool requirePuzzleSolved = false;
    [SerializeField] private BoolReference puzzleSolvedFlag; // 外部コードで更新されるフラグ

    [Header("演出")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private AudioSource doorSE;

    [Header("シーンまたぎ（任意）")]
    [SerializeField] private string nextSceneName = "";     // 空なら同一シーン移動
    [SerializeField] private string nextSpawnPointName = ""; // 次シーン側のSpawn名

    private bool isPlayerInside;
    private bool isOpening;
    private Transform player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInside = true;
        player = other.transform;

        Debug.Log($"[Door2D] Player entered {gameObject.name} (NO rotation)");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerInside = false;
        player = null;
        Debug.Log($"[Door2D] Player exited {gameObject.name}");
    }

    private void Update()
    {
        if (!isPlayerInside || isOpening) return;

        if (Input.GetKeyDown(openKey))
        {
            if (requirePuzzleSolved && (puzzleSolvedFlag == null || !puzzleSolvedFlag.Value))
            {
                Debug.Log("[Door2D] ギミック未解決のため扉は開かない");
                return;
            }
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        isOpening = true;

        if (doorAnimator) doorAnimator.SetTrigger("Open");
        if (doorSE) doorSE.Play();

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            LevelSpawnRouter2D.NextSpawnPointName =
                string.IsNullOrEmpty(nextSpawnPointName) ? "Spawn_Default" : nextSpawnPointName;

            SceneManager.LoadScene(nextSceneName);
            return;
        }
        // ★プレイヤー移動
        if (player == null)
        {
            Debug.LogWarning("[Door2D] player が null。OnTriggerEnter2D が走っていない可能性");
        }
        else if (targetPoint == null)
        {
            Debug.LogWarning("[Door2D] targetPoint 未設定。SpawnPoint をドラッグしてください");
        }
        else
        {
            var rb2d = player.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.linearVelocity = Vector2.zero;
                rb2d.position = (Vector2)targetPoint.position;
            }
            else
            {
                Vector3 p = player.position;
                p.x = targetPoint.position.x;
                p.y = targetPoint.position.y;
                player.position = p;
            }
        }

        Debug.Log($"{gameObject.name} の扉が開き、{(targetPoint ? targetPoint.name : "None")} へテレポート");
    }
}

[System.Serializable]
public class BoolReference
{
    public bool Value;
}
