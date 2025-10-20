using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public TMP_Text slotText;

    private ItemData currentItem;
    private InventoryUI inventoryUI;

    void Start()
    {
        inventoryUI = FindFirstObjectByType<InventoryUI>();
        GetComponent<Button>().onClick.AddListener(OnClickSlot);
    }

    public void SetItem(ItemData item)
    {
        currentItem = item;
        slotText.text = item.itemName;
    }

    public void ClearSlot()
    {
        currentItem = null;
        slotText.text = "";
    }

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

    public void OnClickSlot()
    {
        if (currentItem == null) return;

        //  ����ItemTrigger�Ƃ̘A�g���O���B�A�C�e���ʏ�g�p�̂݁B
        Debug.Log($"[ItemSlot] {currentItem.itemName} ���g�p�I");

        currentItem.Use();

        PauseMenu.Instance.Resume();
        Debug.Log("[ItemSlot] OnClickSlot �I��");
    }
}