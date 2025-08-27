using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData; // Inspector�Ŋ��蓖�Ă�iPotion.asset �Ȃǁj

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �C���x���g���ɒǉ�
            InventoryManager.Instance.AddItem(itemData);

            // ���̃A�C�e�����V�[���������
            Destroy(gameObject);
        }
    }
}
