using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // �V���O���g��

    [Header("UI �Q��")]
    public Transform slotParent; // �X���b�g����ׂ�e�I�u�W�F�N�g�iGrid/Content�j

    private ItemSlot[] slots;

    // �����A�C�e�����X�g�iID�ŊǗ��j
    [System.Serializable]
    public class InventoryItem
    {
        public string itemID;
        public string itemName;
        public string description;
        public bool isConsumable;
        public ItemBehaviour source; // ���̃I�u�W�F�N�g (���ʎ��s�p)
    }

    public List<InventoryItem> items = new List<InventoryItem>();

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

    /// <summary>
    /// �A�C�e���ǉ�
    /// </summary>
    public void AddItem(ItemBehaviour item)
    {
        InventoryItem newItem = new InventoryItem()
        {
            itemID = item.itemID,
            itemName = item.itemName,
            description = item.description,
            source = item
        };

        items.Add(newItem);
        RefreshUI();
    }

    /// <summary>
    /// �A�C�e���폜 (ID�ō폜)
    /// </summary>
    public void RemoveItemByID(string itemID)
    {
        var target = items.Find(i => i.itemID == itemID);
        if (target != null)
        {
            items.Remove(target);
            RefreshUI();
        }
    }

    /// <summary>
    /// �A�C�e���g�p
    /// </summary>
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return;

        var target = items[slotIndex];

        if (target.source != null)
        {
            target.source.Use(); // �� �����Ŋe�A�C�e���̏���������
        }
        else
        {
            Debug.Log(target.itemName + " �͌��ʂ��ݒ肳��Ă��Ȃ�");
        }
    }

    /// <summary>
    /// UI�X�V
    /// </summary>
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