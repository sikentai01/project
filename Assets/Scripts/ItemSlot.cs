using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;  // ISelectHandler ���g�����߂ɕK�v

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public TMP_Text slotText;   // �X���b�g���ɕ\������e�L�X�g

    private ItemData currentItem;
    private InventoryUI inventoryUI;

    void Start()
    {
        // �V�[�����ɂ��� InventoryUI ��T��
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        // ���̃X���b�g���{�^���Ȃ�A�N���b�N���ɌĂ΂��悤�ɐݒ�
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

    // ISelectHandler ���� �� �J�[�\����������u�Ԃ� OnSelectSlot() ���Ă�
    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log("�J�[�\�����X���b�g�ɏ����: " + (currentItem != null ? currentItem.itemName : "��"));
        OnSelectSlot();
    }

    // Enter�i�N���b�N�j�Ŏg�p
    public void OnClickSlot()
    {
        if (currentItem == null) return; // �A�C�e�����Ȃ���Ή������Ȃ�

        Debug.Log(currentItem.itemName + " ���g�p���܂����I");

        // �A�C�e���̌��ʔ���
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