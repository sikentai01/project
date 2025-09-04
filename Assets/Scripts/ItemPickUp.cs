using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // ���̃I�u�W�F�N�g������ ItemBehaviour ���擾
            ItemBehaviour item = GetComponent<ItemBehaviour>();

            if (item != null)
            {
                InventoryManager.Instance.AddItem(item);
                Debug.Log(item.itemName + " �����I");

                Destroy(gameObject); // �}�b�v�������
            }
            else
            {
                Debug.LogWarning("ItemBehaviour �����̃I�u�W�F�N�g�ɖ�����I");
            }
        }
    }
}