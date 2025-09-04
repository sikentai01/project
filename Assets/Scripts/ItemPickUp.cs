using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // このオブジェクトが持つ ItemBehaviour を取得
            ItemBehaviour item = GetComponent<ItemBehaviour>();

            if (item != null)
            {
                InventoryManager.Instance.AddItem(item);
                Debug.Log(item.itemName + " を入手！");

                Destroy(gameObject); // マップから消す
            }
            else
            {
                Debug.LogWarning("ItemBehaviour がこのオブジェクトに無いよ！");
            }
        }
    }
}