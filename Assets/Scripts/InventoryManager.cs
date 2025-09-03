using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<ItemData> items = new List<ItemData>();
    public Transform slotParent;

    private ItemSlot[] slots;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        slots = slotParent.GetComponentsInChildren<ItemSlot>();
        RefreshUI();

        // ▼ テスト用に仮のアイテムを追加
        CreateTestItems();
    }

    public void AddItem(ItemData newItem)
    {
        items.Add(newItem);
        RefreshUI();
    }

    public void RemoveItem(ItemData item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            RefreshUI();
        }
    }

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

    // ▼ テスト用のアイテムを作成
    void CreateTestItems()
    {
        // 消費アイテム
        ItemData testBottle = ScriptableObject.CreateInstance<ItemData>();
        testBottle.itemName = "potion of poison";
        testBottle.description = "A poisonous bottle that will kill you if you drink it.";
        testBottle.isConsumable = true;
        testBottle.effectType = ItemData.EffectType.Poison;
        AddItem(testBottle);

        // 鍵アイテム
        ItemData testKey = ScriptableObject.CreateInstance<ItemData>();
        testKey.itemName = "testkey";
        testKey.description = "A key to open the test door.";
        testKey.isConsumable = false;
        testKey.effectType = ItemData.EffectType.Key;
        testKey.keyID = "DoorA";
        AddItem(testKey);

    }
}