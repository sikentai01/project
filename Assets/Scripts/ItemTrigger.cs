using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    public ItemBehaviour item;       // このトリガーが持ってるアイテム
    public GameObject targetObject;  // 消したい対象（未設定なら自分自身）
    public int requiredDirection = -1; // -1なら判定なし / 0=下,1=左,2=右,3=上

    private bool isPlayerNear = false;
    private GridMovement playerMovement;

    void Start()
    {
        if (targetObject == null) targetObject = gameObject;
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return)) // Enterキー
        {
            if (item != null)
            {
                if (requiredDirection == -1 ||
                    (playerMovement != null && playerMovement.GetDirection() == requiredDirection))
                {
                    InventoryManager.Instance.AddItem(item);
                    Debug.Log(item.itemName + " を入手！");

                    Destroy(targetObject);
                }
                else
                {
                    Debug.Log("向きが違うので拾えない");
                }
            }
            else
            {
                Debug.LogWarning("ItemBehaviour が設定されていません！");
            }
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