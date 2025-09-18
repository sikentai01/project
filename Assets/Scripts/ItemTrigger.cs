using UnityEngine;
using System.Collections.Generic;

public class ItemTrigger : MonoBehaviour
{
    [Header("このトリガー固有のID（セーブ用にユニークに設定）")]
    public string triggerID;

    [Header("拾えるアイテムデータ")]
    public ItemData itemData;

    [Header("見た目を消すオブジェクト（机や松明など）")]
    public GameObject targetObject;

    [Header("必要な向き (0=下,1=左,2=右,3=上, -1=制限なし)")]
    public int requiredDirection = -1;

    [Header("ギミック (任意)")]
    public List<GimmickBase> gimmicks = new List<GimmickBase>();

    [Header("現在の進行度 (セーブ対象)")]
    public int currentStage = 0;

    private bool isPlayerNear = false;
    private GridMovement playerMovement;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // 向きチェック
            if (requiredDirection != -1 && playerMovement != null && playerMovement.GetDirection() != requiredDirection)
            {
                Debug.Log("方向が合っていないのでアイテムを取得できない");
                return;
            }

            // ギミックが残っているならそちらを処理
            if (currentStage < gimmicks.Count)
            {
                Debug.Log($"ギミック {currentStage} 開始");
                gimmicks[currentStage].StartGimmick(this);
            }
            else
            {
                CollectItem();
            }
        }
    }

    // ギミック完了を受け取る
    public void CompleteCurrentGimmick()
    {
        currentStage++;
        Debug.Log($"進行段階が {currentStage} になった");

        // 全部終わったらアイテム入手へ
        if (currentStage >= gimmicks.Count)
        {
            CollectItem();
        }
    }

    // アイテム入手
    public void CollectItem()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            Debug.Log(itemData.itemName + " を入手！");

            if (targetObject != null)
                targetObject.SetActive(false);
            else
                gameObject.SetActive(false); // Destroy じゃなく非表示
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            playerMovement = other.GetComponent<GridMovement>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            playerMovement = null;
        }
    }
}