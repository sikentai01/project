using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // シングルトン

    [Header("UI 参照")]
    public Transform slotParent; // スロットを並べる親オブジェクト（Grid/Content）

    private ItemSlot[] slots;

    // 所持アイテムリスト（IDで管理）
    [System.Serializable]
    public class InventoryItem
    {
        public string itemID;
        public string itemName;
        public string description;
        public bool isConsumable;
        public ItemBehaviour source; // 元のオブジェクト (効果実行用)
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
    /// アイテム追加
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
    /// アイテム削除 (IDで削除)
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
    /// アイテム使用
    /// </summary>
    public void UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count) return;

        var target = items[slotIndex];

        if (target.source != null)
        {
            target.source.Use(); // ← ここで各アイテムの処理が走る
        }
        else
        {
            Debug.Log(target.itemName + " は効果が設定されていない");
        }
    }

    /// <summary>
    /// UI更新
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