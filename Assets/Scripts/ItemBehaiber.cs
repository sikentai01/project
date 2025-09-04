using UnityEngine;
using System.Collections.Generic;

public class ItemBehaviour : MonoBehaviour
{
    public string itemID;
    public string itemName;
    [TextArea] public string description;

    [Header("効果リスト（複数アタッチ可）")]
    public List<ItemEffect> effects = new List<ItemEffect>();

    public void Use()
    {
        if (effects.Count == 0)
        {
            Debug.Log(itemName + " は効果が設定されていない");
            return;
        }

        foreach (var effect in effects)
        {
            effect.Execute(this);
        }
    }

    public void Consume()
    {
        InventoryManager.Instance.RemoveItemByID(itemID);
    }
}