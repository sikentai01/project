using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SaveTrigger : MonoBehaviour
{
    [Header("サウンド設定")]
    [SerializeField] private AudioClip eventBGM;

    private bool isPlayerNear = false;
    private GridMovement player;

    [Header("必要な方向 (0=下, 1=左, 2=右, 3=上, -1=制限なし)")]
    public int requiredDirection = 3;

    [Header("会話イベントで渡すアイテム")]
    public ItemData rewardItem;

    private Light2D normalLight;
    private Light2D restrictedLight;

    [Header("NPC関連（シーン内の仮置き用）")]
    public GameObject sceneNpc;
    public Vector2 npcSpawnPosition;

    [Header("1回きりにするか")]
    public bool oneTimeOnly = true;
    private bool alreadyTriggered = false;

    void OnEnable()
    {
        if (GameFlags.Instance != null && !GameFlags.Instance.HasFlag("SaveTriggered"))
        {
            alreadyTriggered = false;
            Debug.Log("[SaveTrigger] フラグ未設定のため再有効化");
        }
    }

    void Start()
    {
        player = FindFirstObjectByType<GridMovement>();

        if (player != null)
        {
            normalLight = player.GetComponent<Light2D>();
            var childLights = player.GetComponentsInChildren<Light2D>(true);
            foreach (var l in childLights)
            {
                if (l.name == "RestrictedLight")
                    restrictedLight = l;
            }

            if (restrictedLight != null) restrictedLight.enabled = true;
            if (normalLight != null) normalLight.enabled = false;
        }

        if (sceneNpc != null) sceneNpc.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            if (oneTimeOnly && alreadyTriggered) return;

            if (requiredDirection == -1 || player.GetDirection() == requiredDirection)
            {
                StartCoroutine(EventFlow());
            }
            else
            {
                Debug.Log("向きが違うので調べられない");
            }
        }
    }

    private IEnumerator EventFlow()
    {
        alreadyTriggered = true;
        Debug.Log("[SaveTrigger] イベント開始");

        // ---- BGM再生 ----
        if (SoundManager.Instance != null && eventBGM != null)
            SoundManager.Instance.PlayBGM(eventBGM);

        // ---- ライト切り替え ----
        if (restrictedLight != null) restrictedLight.enabled = false;
        if (normalLight != null) normalLight.enabled = true;

        // ---- プレイヤー停止 & メニュー禁止 ----
        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;
        player?.SetDirection(0);

        // ---- NPC出現 ----
        if (sceneNpc != null)
        {
            sceneNpc.transform.position = npcSpawnPosition;
            sceneNpc.SetActive(true);
        }

        // ----  会話イベントを再生 ----
        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire("talk_001");
            yield return new WaitUntil(() => !IsConversationActive());
        }

        // ---- 会話終了後にアイテム渡す ----
        if (rewardItem != null)
        {
            InventoryManager.Instance.AddItem(rewardItem);
            Debug.Log($"[SaveTrigger] アイテム『{rewardItem.itemName}』を入手！");
        }

        // ---- NPC消える ----
        if (sceneNpc != null) sceneNpc.SetActive(false);

        // ---- 落とし穴無効化 ----
        var trap = FindFirstObjectByType<FallTrap>();
        if (trap != null) trap.DisableTrap();

        // ---- 復帰処理 ----
        GameFlags.Instance.SetFlag("SaveTriggered");
        if (player != null) player.enabled = true;
        PauseMenu.blockMenu = false;

        if (SoundManager.Instance != null)
            SoundManager.Instance.StopBGM();

        Debug.Log("[SaveTrigger] イベント終了");
    }

    private bool IsConversationActive()
    {
        var core = FindObjectOfType<DialogueCore>();
        return core != null && core.enabled;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNear = false;
    }
}