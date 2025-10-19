using System.Collections.Generic;
using UnityEngine;

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

    public bool IsPlayerNear => isPlayerNear;

    void Update()
    {
        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // 向きチェック
            if (requiredDirection != -1 && playerMovement != null && playerMovement.GetDirection() != requiredDirection)
            {
                Debug.Log("方向が合っていないのでアイテムを取得できない");
                return;
            }

            // ギミックが残っている場合
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

    // ギミック完了時
    public void CompleteCurrentGimmick()
    {
        currentStage++;
        Debug.Log($"進行段階が {currentStage} になった");

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
                gameObject.SetActive(false);
        }
    }

    //  このトリガーが「アイテム使用に対応しているか」を判定
    public bool HasPendingGimmick(ItemData item)
    {
        if (currentStage < gimmicks.Count)
        {
            var gimmick = gimmicks[currentStage];
            Debug.Log($"[HasPendingGimmick] {gimmick.name} で {item.itemName} を確認中");

            if (!gimmick.NeedsItem) return false;
            bool result = gimmick.CanUseItem(item);
            Debug.Log($"[HasPendingGimmick] 判定結果 = {result}");
            return result;
        }
        return false;
    }

    //  アイテムをギミックに使用
    public void UseItemOnGimmick(ItemData item)
    {
        if (!isPlayerNear) return;

        if (requiredDirection != -1 && playerMovement != null && playerMovement.GetDirection() != requiredDirection)
            return;

        if (currentStage < gimmicks.Count)
        {
            var gimmick = gimmicks[currentStage];

            if (gimmick.NeedsItem)
            {
                if (gimmick.CanUseItem(item))
                {
                    gimmick.UseItem(item, this);
                }
                else
                {
                    Debug.Log("このアイテムは使えません");
                }
            }
            else
            {
                gimmick.StartGimmick(this); // アイテム不要ギミック
            }
        }
        else
        {
            CollectItem();
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
