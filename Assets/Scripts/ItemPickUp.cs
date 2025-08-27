using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData; // Inspectorで割り当てる（Potion.asset など）

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // インベントリに追加
            InventoryManager.Instance.AddItem(itemData);

            // このアイテムをシーンから消す
            Destroy(gameObject);
        }
    }
}
