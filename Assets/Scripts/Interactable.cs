using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string message = "���̒��𒲂ׂ�";
    public ItemData hiddenItem; // ���Ȃǂ̃A�C�e��
    private bool isPlayerNear = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E)) // E�L�[�Œ��ׂ�
        {
            // ���b�Z�[�W���o���iUI�ɕ\������Ȃǁj
            Debug.Log(message);

            // �A�C�e�����ݒ肳��Ă��������
            if (hiddenItem != null)
            {
                InventoryManager.Instance.AddItem(hiddenItem);
                Debug.Log(hiddenItem.itemName + " ����ɓ��ꂽ�I");
                hiddenItem = null; // ��x����ɂ���ꍇ�͏���
            }
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
