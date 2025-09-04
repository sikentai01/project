using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public TMP_Text slotText;   // �X���b�g���ɕ\������e�L�X�g

    private InventoryManager.InventoryItem currentItem;
    private InventoryUI inventoryUI;

    void Start()
    {
        // �V�[�����ɂ��� InventoryUI ��T��
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        // ���̃X���b�g���{�^���Ȃ�A�N���b�N���ɌĂ΂��悤�ɐݒ�
        GetComponent<Button>().onClick.AddListener(OnClickSlot);
    }

    // �A�C�e�����Z�b�g���ĕ\��
    public void SetItem(InventoryManager.InventoryItem item)
    {
        currentItem = item;
        slotText.text = item.itemName; // ���O��\��
    }

    // �A�C�e�����Ȃ��ꍇ�̓N���A
    public void ClearSlot()
    {
        currentItem = null;
        slotText.text = "";
    }

    // ���̃X���b�g��I�񂾂Ƃ��ɐ��������X�V
    public void OnSelectSlot()
    {
        if (currentItem != null)
            inventoryUI.ShowDescription(new ItemData
            {
                itemName = currentItem.itemName,
                description = currentItem.description
            });
        else
            inventoryUI.ShowDescription(null);
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelectSlot();
    }

    // Enter�i�N���b�N�j�Ŏg�p
    public void OnClickSlot()
    {
        if (currentItem == null) return;

        Debug.Log(currentItem.itemName + " ���g�p���܂����I");

        // InventoryManager�ɏ����𓊂���
        int index = transform.GetSiblingIndex();
        InventoryManager.Instance.UseItem(index);
    }
}