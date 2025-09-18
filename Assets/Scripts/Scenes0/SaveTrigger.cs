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

    private Light2D normalLight;
    private Light2D restrictedLight;

    void Start()
    {
        player = FindFirstObjectByType<GridMovement>();

        if (player != null)
        {
            // Normal は Yuki 本体のコンポーネントから取得
            normalLight = player.GetComponent<Light2D>();

            // Restricted は Yuki の子オブジェクトから取得
            var childLights = player.GetComponentsInChildren<Light2D>(true);
            foreach (var l in childLights)
            {
                if (l.name == "RestrictedLight") // ← Unity上の名前に合わせる
                {
                    restrictedLight = l;
                }
            }

            // シーン0開始時は狭いライトだけONに
            if (restrictedLight != null) restrictedLight.enabled = true;
            if (normalLight != null) normalLight.enabled = false;
        }
    }
    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
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
        Debug.Log("セーブ調べた");
        //  このタイミングでライト切り替え
        if (restrictedLight != null) restrictedLight.enabled = false;
        if (normalLight != null) normalLight.enabled = true;

        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;
        if (player != null) player.SetDirection(0);

        yield return new WaitForSeconds(1.5f);

        Debug.Log("キャラが現れた: 『よく来たな』");

        yield return new WaitForSeconds(3f);

        if (rewardItem != null)
        {
            InventoryManager.Instance.AddItem(rewardItem);
            Debug.Log($"キャラからアイテム『{rewardItem.itemName}』を受け取った！");
        }

        yield return new WaitForSeconds(3f);

        Debug.Log("キャラ: 『ではまた会おう…』");

        yield return new WaitForSeconds(2f);

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