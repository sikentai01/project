using UnityEngine;

public class Door : MonoBehaviour
{
    public string doorID; // ���̃h�A���J���邽�߂̌�ID
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            foreach (var item in InventoryManager.Instance.items)
            {
                // ���A�C�e������
                if (item.itemID == doorID)
                {
                    Debug.Log(item.itemName + " ���g���ăh�A���J�����I");

                    // ���Օi�Ȃ�폜
                    if (item.isConsumable)
                        InventoryManager.Instance.RemoveItemByID(item.itemID);

                    // �h�A�폜�i�����̓A�j���[�V�����ɍ����ւ����j
                    Destroy(gameObject);
                    return;
                }
            }

            Debug.Log("�Ή����錮�������Ă��܂���");
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