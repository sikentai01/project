using UnityEngine;
using System.Collections.Generic;

public class ItemTrigger : MonoBehaviour
{
    [Header("拾えるアイテムデータ")]
    public ItemData itemData;   // ScriptableObject をアタッチする
    [Header("見た目を消すオブジェクト（机や松明など）")]
    public GameObject targetObject;
    [Header("必要な向き (0=下,1=左,2=右,3=上, -1=制限なし)")]
    public int requiredDirection = -1; // -1ならどの向きでもOK

    [Header("ギミック (任意)")]
    public List<GimmickBase> gimmicks = new List<GimmickBase>();

    private int currentGimmickIndex = 0;
    private bool isPlayerNear = false;
    private GridMovement playerMovement;

    // ★ ギミックが終わった後「待機中」かどうか
    private bool waitingForNext = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return)) // Enterで進行
        {
            // 向きチェック
            if (requiredDirection != -1 && playerMovement != null && playerMovement.GetDirection() != requiredDirection)
            {
                Debug.Log("方向が合っていないのでアイテムを取得できない");
                return;
            }

            if (waitingForNext)
            {
                // 次に進む処理
                waitingForNext = false;
                ContinueGimmickOrItem();
                return;
            }

            // ギミック開始
            if (currentGimmickIndex < gimmicks.Count)
            {
                gimmicks[currentGimmickIndex].StartGimmick(this);
            }
            else
            {
                CollectItem();
            }
        }
    }

    public void CompleteCurrentGimmick()
    {
        Debug.Log("ギミック完了！次の入力を待っています...");
        waitingForNext = true; // ← 次のEnterを待つ
    }

    private void ContinueGimmickOrItem()
    {
        currentGimmickIndex++;
        if (currentGimmickIndex < gimmicks.Count)
        {
            gimmicks[currentGimmickIndex].StartGimmick(this);
        }
        else
        {
            CollectItem();
        }
    }

    public void CollectItem()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            Debug.Log(itemData.itemName + " を入手！");

            if (targetObject != null)
                targetObject.SetActive(false);
            else
                Destroy(gameObject);
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