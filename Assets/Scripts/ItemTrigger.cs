using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    public ItemBehaviour item;       // ���̃g���K�[�������Ă�A�C�e��
    public GameObject targetObject;  // ���������Ώہi���ݒ�Ȃ玩�����g�j
    public int requiredDirection = -1; // -1�Ȃ画��Ȃ� / 0=��,1=��,2=�E,3=��

    private bool isPlayerNear = false;
    private GridMovement playerMovement;

    void Start()
    {
        if (targetObject == null) targetObject = gameObject;
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return)) // Enter�L�[
        {
            if (item != null)
            {
                if (requiredDirection == -1 ||
                    (playerMovement != null && playerMovement.GetDirection() == requiredDirection))
                {
                    InventoryManager.Instance.AddItem(item);
                    Debug.Log(item.itemName + " �����I");

                    Destroy(targetObject);
                }
                else
                {
                    Debug.Log("�������Ⴄ�̂ŏE���Ȃ�");
                }
            }
            else
            {
                Debug.LogWarning("ItemBehaviour ���ݒ肳��Ă��܂���I");
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