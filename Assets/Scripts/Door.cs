using UnityEngine;

public class Door : MonoBehaviour
{
    public string doorID; // このドアを開けるための鍵ID
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            foreach (var item in InventoryManager.Instance.items)
            {
                // 鍵アイテム判定
                if (item.itemID == doorID)
                {
                    Debug.Log(item.itemName + " を使ってドアを開けた！");

                    // 消耗品なら削除
                    if (item.isConsumable)
                        InventoryManager.Instance.RemoveItemByID(item.itemID);

                    // ドア削除（ここはアニメーションに差し替えも可）
                    Destroy(gameObject);
                    return;
                }
            }

            Debug.Log("対応する鍵を持っていません");
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