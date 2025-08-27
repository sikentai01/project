using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string message = "机の中を調べた";
    public ItemData hiddenItem; // 鍵などのアイテム
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E)) // Eキーで調べる
        {
            // メッセージを出す（UIに表示するなど）
            Debug.Log(message);

            // アイテムが設定されていたら入手
            if (hiddenItem != null)
            {
                InventoryManager.Instance.AddItem(hiddenItem);
                Debug.Log(hiddenItem.itemName + " を手に入れた！");
                hiddenItem = null; // 一度きりにする場合は消す
            }
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
