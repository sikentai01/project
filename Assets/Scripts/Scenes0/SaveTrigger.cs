using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SaveTrigger : MonoBehaviour, ISceneInitializable
{
    [Header("サウンド設定")]
    [SerializeField] private AudioClip eventBGM;

    private bool isPlayerNear = false;
    private GridMovement player;

    [Header("必要な方向 (0=下, 1=左, 2=右, 3=上, -1=制限なし)")]
    public int requiredDirection = 3;

    [Header("会話イベントで渡すアイテム")]
    public ItemData rewardItem;

    [Header("NPC関連（シーン内の仮置き用）")]
    public GameObject sceneNpc;
    public Vector2 npcSpawnPosition;

    [Header("1回きりにするか")]
    public bool oneTimeOnly = true;
    private bool alreadyTriggered = false;

    private const string FLAG_ID = "SaveTriggered";

    private void OnEnable()
    {
        StartCoroutine(DeferredFlagSync());
    }

    private IEnumerator DeferredFlagSync()
    {
        yield return null; // GameBootstrapのロード完了を1フレーム待つ
        if (GameFlags.Instance != null)
        {
            alreadyTriggered = GameFlags.Instance.HasFlag(FLAG_ID);
            Debug.Log($"[SaveTrigger] DeferredFlagSync: {FLAG_ID}={alreadyTriggered}");
        }
    }

    private void Start()
    {
        InitializeTrigger();
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            if (oneTimeOnly && alreadyTriggered) return;

            if (requiredDirection == -1 || player.GetDirection() == requiredDirection)
                StartCoroutine(EventFlow());
            else
                Debug.Log("[SaveTrigger] 向きが違うので調べられない");
        }
    }

    private IEnumerator EventFlow()
    {
        alreadyTriggered = true;
        Debug.Log("[SaveTrigger] イベント開始");

        SoundManager.Instance?.PlayBGM(eventBGM);

        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;
        player.SetDirection(0);

        if (sceneNpc != null)
        {
            sceneNpc.transform.position = npcSpawnPosition;
            sceneNpc.SetActive(true);
        }

        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire("talk_001");
            yield return new WaitUntil(() => !IsConversationActive());
        }

        if (rewardItem != null && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(rewardItem);
            Debug.Log($"[SaveTrigger] アイテム『{rewardItem.itemName}』を入手！");
        }

        if (sceneNpc != null) sceneNpc.SetActive(false);

        var trap = FindFirstObjectByType<FallTrap>();
        trap?.DisableTrap();

        GameFlags.Instance?.SetFlag(FLAG_ID);

        if (player != null) player.enabled = true;
        PauseMenu.blockMenu = false;
        SoundManager.Instance?.StopBGM();

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

    private void InitializeTrigger()
    {
        player = FindFirstObjectByType<GridMovement>();
        if (player == null) return;

        if (sceneNpc != null) sceneNpc.SetActive(false);

        Debug.Log($"[SaveTrigger] 初期化完了 (Triggered={alreadyTriggered})");
    }

    public void InitializeSceneAfterLoad()
    {
        Debug.Log("[SaveTrigger] InitializeSceneAfterLoad 呼び出し");
        InitializeTrigger();
    }
}