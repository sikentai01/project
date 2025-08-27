using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public TMP_Text slotText;   // �X���b�g���ɕ\������e�L�X�g
    public Image slotIcon;      // �A�C�R���\���i�s�v�Ȃ�Inspector�ŊO���j

    private ItemData currentItem;
    private InventoryUI inventoryUI;

    void Start()
    {
        // �V�[�����ɂ��� InventoryUI ��T��
        inventoryUI = inventoryUI = FindFirstObjectByType<InventoryUI>();

        // ���̃X���b�g���{�^���Ȃ�A�N���b�N���ɌĂ΂��悤�ɐݒ�
        GetComponent<Button>().onClick.AddListener(OnClickSlot);
    }

    // �A�C�e�����Z�b�g���ĕ\��
    public void SetItem(ItemData item)
    {
        currentItem = item;
        slotText.text = item.itemName;

        if (slotIcon) slotIcon.sprite = item.icon;
    }

    // �A�C�e�����Ȃ��ꍇ�̓N���A
    public void ClearSlot()
    {
        currentItem = null;
        slotText.text = "";
        if (slotIcon) slotIcon.sprite = null;
    }

    // ���̃X���b�g��I�񂾂Ƃ��ɐ��������X�V
    void OnClickSlot()
    {
        if (currentItem != null)
        {
            inventoryUI.ShowDescription(currentItem);
        }
    }
}
