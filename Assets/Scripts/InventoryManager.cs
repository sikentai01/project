using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public List<string> ownedItemIDs = new List<string>();
}

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

        // �� DebugOwned �`�F�b�N
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
        if (!items.Contains(item))
        {
            items.Add(item);
            RefreshUI();
        }
    }

    /// <summary> �A�C�e���폜 </summary>
    public void RemoveItemByID(string itemID)
    {
        var target = items.Find(i => i.itemID == itemID);
        if (target != null)
        {
            items.Remove(target);
            RefreshUI();
        }
    }

    /// <summary> UI�X�V </summary>
    void RefreshUI()
    {
        if (slots == null) return;
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < items.Count)
                slots[i].SetItem(items[i]);
            else
                slots[i].ClearSlot();
        }
    }

    public bool HasItem(string itemID)
    {
        foreach (var item in items)
        {
            if (item.itemID == itemID)
                return true;
        }
        return false;
    }

    // ===== �Z�[�u�f�[�^�쐬 =====
    public InventorySaveData SaveData()
    {
        var data = new InventorySaveData();

        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.itemID))
                data.ownedItemIDs.Add(item.itemID);
        }

        Debug.Log($"[InventoryManager] {data.ownedItemIDs.Count} ���̃A�C�e����ۑ����܂���");
        return data;
    }

    // ===== �Z�[�u�f�[�^�ǂݍ��� =====
    public void LoadData(InventorySaveData data)
    {
        items.Clear();

        foreach (var id in data.ownedItemIDs)
        {
            var found = allItems.Find(i => i.itemID == id);
            if (found != null)
                items.Add(found);
            else
                Debug.LogWarning($"[InventoryManager] ID {id} �̃A�C�e����������܂���");
        }

        RefreshUI();
        Debug.Log("[InventoryManager] �C���x���g���f�[�^�����[�h���܂���");
    }

    // ===== ������ =====
    public void ClearAll()
    {
        items.Clear();
        RefreshUI();
        Debug.Log("[InventoryManager] �C���x���g�������������܂���");
    }

}
