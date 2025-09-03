using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public TMP_Text slotText;   // �X���b�g�ɕ\�����閼�O
    private ItemData currentItem;
    private InventoryUI inventoryUI;

    void Start()
    {
        // �V�[�����ɂ��� InventoryUI ��T��
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        // �{�^���̃N���b�N�C�x���g�ɓo�^
        GetComponent<Button>().onClick.AddListener(OnClickSlot);
    }

    // �A�C�e�����Z�b�g���ĕ\��
    public void SetItem(ItemData item)
    {
        currentItem = item;
        slotText.text = item.itemName;
    }

    // �A�C�e�����Ȃ��ꍇ�̓N���A
    public void ClearSlot()
    {
        currentItem = null;
        slotText.text = "";
    }

    // �J�[�\�����������Ƃ��ɏ���\��
    public void OnSelectSlot()
    {
        if (currentItem != null)
            inventoryUI.ShowDescription(currentItem);
        else
            inventoryUI.ShowDescription(null);
    }

    // Enter�������Ƃ��Ɏg�p
    public void OnClickSlot()
    {
        if (currentItem == null) return;

        Debug.Log(currentItem.itemName + " ���g�p���܂����I");

        currentItem.UseEffect();

        // ���Օi�Ȃ�폜
        if (currentItem.isConsumable)
        {
            InventoryManager.Instance.RemoveItem(currentItem);
        }

        // ���j���[�����
        FindFirstObjectByType<PauseMenu>().Resume();
    }
}