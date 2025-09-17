using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("基本情報")]
    public string itemID;
    public string itemName;
    [TextArea] public string description;
    public bool isConsumable;

    [Header("デバッグ用")]
    public bool DebugOwned;   // ← これを追加

    [Header("効果リスト")]
    [SerializeField] private List<ItemEffect> effects = new List<ItemEffect>();
    public List<ItemEffect> Effects => effects;

    public void Use()
    {
        if (effects == null || effects.Count == 0)
        {
            Debug.Log(itemName + " は効果が設定されていない");
            return;
        }

        foreach (var effect in effects)
        {
            effect.Execute(this);
        }

        if (isConsumable)
        {
            Debug.Log(itemName + " を消費した");
            InventoryManager.Instance.RemoveItemByID(itemID);
        }
    }
}