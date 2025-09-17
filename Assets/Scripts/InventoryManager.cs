using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI �Q��")]
    public Transform slotParent;
    private ItemSlot[] slots;

    // �����A�C�e�����X�g
    public List<ItemData> items = new List<ItemData>();

    // Debug�p�ɍŏ����玝���Ă�A�C�e��
    [Header("�S�A�C�e�����X�g�i�f�o�b�O�p�j")]
    public List<ItemData> allItems = new List<ItemData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        slots = slotParent.GetComponentsInChildren<ItemSlot>();

        // �� ������ DebugOwned �`�F�b�N
        foreach (var item in allItems)
        {
            if (item != null && item.DebugOwned && !items.Contains(item))
            {
                items.Add(item);
                Debug.Log(item.itemName + " ���f�o�b�O�����Ƃ��Ēǉ�");
            }
        }

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
        var target = items.Find(i => i.itemID == itemID); // �� �C���ς�: itemID �Ō���
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
            target.Use(); // ScriptableObject �� Use()
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