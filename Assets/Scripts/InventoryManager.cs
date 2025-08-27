using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; // シングルトン

    public List<ItemData> items = new List<ItemData>(); // 入手したアイテムリスト
    public Transform slotParent; // スロットを並べる親オブジェクト（Grid/Content）

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


    // アイテムを追加
    public void AddItem(ItemData newItem)
    {
        items.Add(newItem);
        RefreshUI();
    }

    // スロットUIを更新
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
