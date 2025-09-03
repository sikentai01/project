using UnityEngine;

public class Door : MonoBehaviour
{
    public string doorID;
    private bool isPlayerNear = false;

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            // �C���x���g���̃A�C�e�����m�F
            foreach (var item in InventoryManager.Instance.items)
            {
                if (item.effectType == ItemData.EffectType.Key && item.keyID == doorID)
                {
                    Debug.Log(item.itemName + " �� " + doorID + " ���J�����I");
                    InventoryManager.Instance.RemoveItem(item);
                    Destroy(gameObject); // �h�A�������i�A�j���Đ��ɒu�������j
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