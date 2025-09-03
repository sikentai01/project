using UnityEngine;

public class Door : MonoBehaviour
{
    public string doorID;
    private bool isPlayerNear = false;

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            // インベントリのアイテムを確認
            foreach (var item in InventoryManager.Instance.items)
            {
                if (item.effectType == ItemData.EffectType.Key && item.keyID == doorID)
                {
                    Debug.Log(item.itemName + " で " + doorID + " を開けた！");
                    InventoryManager.Instance.RemoveItem(item);
                    Destroy(gameObject); // ドアを消す（アニメ再生に置き換え可）
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