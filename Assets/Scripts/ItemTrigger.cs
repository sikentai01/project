using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [Header("拾えるアイテムデータ")]
    public ItemData itemData;   // ScriptableObject をアタッチする
    [Header("見た目を消すオブジェクト（机や松明など）")]
    public GameObject targetObject;

    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return)) // Eキーで取得
        {
            if (itemData != null)
            {
                InventoryManager.Instance.AddItem(itemData);
                Debug.Log(itemData.itemName + " を手に入れた！");

                // 見た目を消す
                if (targetObject != null)
                {
                    targetObject.SetActive(false);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                Debug.LogWarning("ItemData が設定されていません！");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            Debug.Log("プレイヤーがアイテムの近くに来た");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            Debug.Log("プレイヤーがアイテムから離れた");
        }
    }
}