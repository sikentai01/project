using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // �V���O���g��

    public List<ItemData> items = new List<ItemData>(); // ���肵���A�C�e�����X�g
    public Transform slotParent; // �X���b�g����ׂ�e�I�u�W�F�N�g�iGrid/Content�j

    private ItemSlot[] slots;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        RefreshUI();
    }


    // �A�C�e����ǉ�
    public void AddItem(ItemData newItem)
    {
        items.Add(newItem);
        RefreshUI();
    }

    // �X���b�gUI���X�V
    void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
                slots[i].SetItem(items[i]);
            else
                slots[i].ClearSlot();
        }
    }
}
