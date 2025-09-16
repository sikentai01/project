using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI �Q��")]
    public Transform slotParent;
    private ItemSlot[] slots;

    // �����A�C�e�����X�g �� ScriptableObject �Q�Ƃɐ؂�ւ�
    public List<ItemData> items = new List<ItemData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        RefreshUI();
    }

    /// <summary> �A�C�e���ǉ� </summary>
    public void AddItem(ItemData item)
    {
        if (!items.Contains(item)) // �d���o�^�h�~
        {
            items.Add(item);
            RefreshUI();
        }
    }

    /// <summary> �A�C�e���폜 (ID �ō폜) </summary>
    public void RemoveItemByID(string itemID)
    {
        var target = items.Find(i => i.name == itemID); // name �� ID ����Ɏg���Ă�
        if (target != null)
        {
            items.Remove(target);
            RefreshUI();
        }
    }

    /// <summary> �A�C�e���g�p </summary>
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return;

        var target = items[slotIndex];
        if (target != null)
        {
            target.Use(); // �� ScriptableObject �� Use() ���Ă΂��
        }
    }

    /// <summary> UI �X�V </summary>
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