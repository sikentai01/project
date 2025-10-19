using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SaveTrigger : MonoBehaviour
{
    [Header("サウンド設定")]
    [SerializeField] private AudioClip eventBGM; // イベント中に流すBGM

    private bool isPlayerNear = false;
    private GridMovement player;

    [Header("必要な方向 (0=下, 1=左, 2=右, 3=上, -1=制限なし)")]
    public int requiredDirection = 3;

    [Header("会話イベントで渡すアイテム")]
    public ItemData rewardItem;

    // ライト（Inspectorでドラッグ不要、Startで自動取得）
    private Light2D normalLight;
    private Light2D restrictedLight;

    [Header("NPC関連（シーン内の仮置き用）")]
    public GameObject sceneNpc;        // シーン内に置いた仮NPC
    public Vector2 npcSpawnPosition;   // Inspectorで直接座標入力

    // 将来的にPrefabでやる場合
    // public GameObject npcPrefab;

    [Header("1回きりにするか")]
    public bool oneTimeOnly = true;    // trueなら1回限り
    private bool alreadyTriggered = false;

    void OnEnable()
    {
        // GameFlagsが初期化済みで「SaveTriggered」フラグが無ければリセット
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

        // ★ はじめから時の再有効化チェック！
        if (!GameFlags.Instance.HasFlag("SaveTriggered"))
        {
            alreadyTriggered = false;
            Debug.Log("[SaveTrigger] フラグ未発動なので再使用可能にしました。");
        }
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
        if (SoundManager.Instance != null && eventBGM != null)
        {
            SoundManager.Instance.PlayBGM(eventBGM);
        }

        alreadyTriggered = true; // 1回限りにする場合はここでロック

        Debug.Log("セーブ調べた");

        // ライト切り替え
        if (restrictedLight != null) restrictedLight.enabled = false;
        if (normalLight != null) normalLight.enabled = true;

        // プレイヤー停止 & メニュー禁止
        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;

        // プレイヤーの向きを下に固定
        if (player != null) player.SetDirection(0);

        // NPC登場
        if (sceneNpc != null)
        {
            sceneNpc.transform.position = npcSpawnPosition;
            sceneNpc.SetActive(true);
        }
        // 将来的にPrefabを使うならこうする
        // if (npcPrefab != null) Instantiate(npcPrefab, npcSpawnPosition, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        Debug.Log("キャラが現れた: 『よく来たな』");

        yield return new WaitForSeconds(3f);

        // アイテム入手
        if (rewardItem != null)
        {
            InventoryManager.Instance.AddItem(rewardItem);
            Debug.Log($"キャラからアイテム『{rewardItem.itemName}』を受け取った！");
        }

        yield return new WaitForSeconds(3f);

        Debug.Log("キャラ: 『ではまた会おう…』");
        if (sceneNpc != null) sceneNpc.SetActive(false);

        yield return new WaitForSeconds(2f);

        GameFlags.Instance.SetFlag("SaveTriggered");

        // シーン内のすべての落とし穴を無効化
        var trap = FindFirstObjectByType<FallTrap>();
        if (trap != null)
        {
            trap.DisableTrap();
        }

        // プレイヤー復帰 & メニュー解禁
        if (player != null) player.enabled = true;
        PauseMenu.blockMenu = false;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBGM();
        }
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