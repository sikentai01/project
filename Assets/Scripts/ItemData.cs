using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("基本情報")]
    public string itemID;           // ← ID を追加
    public string itemName;
    [TextArea] public string description;
    public bool isConsumable;

    [Header("効果リスト (ScriptableObject 参照)")]
    [SerializeField] private List<ItemEffect> effects = new List<ItemEffect>();
    public List<ItemEffect> Effects => effects;

    /// <summary>
    /// アイテムを使用したときの処理
    /// </summary>
    public void Use()
    {
        if (effects == null || effects.Count == 0)
        {
            Debug.Log(itemName + " は効果が設定されていない");
            return;
        }

        foreach (var effect in effects)
        {
            effect.Execute(this); // 自分を渡して実行
        }

        if (isConsumable)
        {
            Debug.Log(itemName + " を消費した！");
            InventoryManager.Instance.RemoveItemByID(itemID);
        }
    }
}