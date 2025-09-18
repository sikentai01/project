using UnityEngine;
using System.Collections;

public class SaveTrigger : MonoBehaviour
{
    private bool isPlayerNear = false;
    private GridMovement player;

    [Header("必要な方向 (0=下, 1=左, 2=右, 3=上, -1=制限なし)")]
    public int requiredDirection = 3; // デフォルト: 上向き

    [Header("会話イベントで渡すアイテム")]
    public ItemData rewardItem;   // Inspectorで設定する

    void Start()
    {
        player = FindFirstObjectByType<GridMovement>();
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

        // 1. プレイヤーの動きを止める & メニュー禁止
        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;

        // 向きを下に固定
        if (player != null) player.SetDirection(0);

        yield return new WaitForSeconds(1.5f);

        // 2. 会話ログ（キャラ出現）
        Debug.Log("キャラが現れた: 『よく来たな』");

        yield return new WaitForSeconds(3f);

        // 3. アイテム入手
        if (rewardItem != null)
        {
            InventoryManager.Instance.AddItem(rewardItem);
            Debug.Log($"キャラからアイテム『{rewardItem.itemName}』を受け取った！");
        }
        else
        {
            Debug.LogWarning("rewardItem が設定されていません");
        }

        yield return new WaitForSeconds(3f);

        // 4. 会話終了
        Debug.Log("キャラ: 『ではまた会おう…』");

        yield return new WaitForSeconds(2f);

        // 5. プレイヤーの動きを戻す & メニュー解禁
        if (player != null) player.enabled = true;
        PauseMenu.blockMenu = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}