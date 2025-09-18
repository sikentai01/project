using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SaveTrigger : MonoBehaviour
{
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

    void Start()
    {
        player = FindFirstObjectByType<GridMovement>();

        if (player != null)
        {
            // NormalLight はプレイヤー本体に付いてる
            normalLight = player.GetComponent<Light2D>();

            // RestrictedLight はプレイヤーの子から取得
            var childLights = player.GetComponentsInChildren<Light2D>(true);
            foreach (var l in childLights)
            {
                if (l.name == "RestrictedLight")
                    restrictedLight = l;
            }

            // 開始時は Restricted だけON
            if (restrictedLight != null) restrictedLight.enabled = true;
            if (normalLight != null) normalLight.enabled = false;
        }

        // 仮置きNPCは最初非表示
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

        // プレイヤー復帰 & メニュー解禁
        if (player != null) player.enabled = true;
        PauseMenu.blockMenu = false;
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