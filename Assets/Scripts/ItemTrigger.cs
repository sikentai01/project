using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [Header("�E����A�C�e���f�[�^")]
    public ItemData itemData;   // ScriptableObject ���A�^�b�`����
    [Header("�����ڂ������I�u�W�F�N�g�i���⏼���Ȃǁj")]
    public GameObject targetObject;

    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return)) // E�L�[�Ŏ擾
        {
            if (itemData != null)
            {
                InventoryManager.Instance.AddItem(itemData);
                Debug.Log(itemData.itemName + " ����ɓ��ꂽ�I");

                // �����ڂ�����
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
                Debug.LogWarning("ItemData ���ݒ肳��Ă��܂���I");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            Debug.Log("�v���C���[���A�C�e���̋߂��ɗ���");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            Debug.Log("�v���C���[���A�C�e�����痣�ꂽ");
        }
    }
}