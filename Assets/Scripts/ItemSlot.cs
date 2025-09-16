using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public TMP_Text slotText;   // �X���b�g���ɕ\������e�L�X�g

    private ItemData currentItem;  // �� InventoryItem �ł͂Ȃ� ItemData �ɕύX
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
            inventoryUI.ShowDescription(currentItem);
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

        // �����𖞂����ꍇ�����m�F�p�l�����o��
        if (CanUseItem(currentItem))
        {
            // ���̃p�l����S������iPauseMenu�ɔC���Ă�OK�j
            PauseMenu.Instance.CloseAllPanels();

            // �m�F�p�l�����J��
            ConfirmPanel.Instance.Show(currentItem);
        }
        else
        {
            Debug.Log(currentItem.itemName + " �͍��g���Ȃ��I");
        }
    }

    private bool CanUseItem(ItemData item)
    {
        // ��������������ɏ���
        // ��F�h�A�̑O����Ȃ��ƌ��͎g���Ȃ� �Ƃ�
        return true;
    }
}