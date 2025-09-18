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

        bool executed = false;

        foreach (var effect in effects)
        {
            if (effect.CanExecute(this))  // ← 条件をアイテムの効果に委ねる
            {
                effect.Execute(this);      // 実行
                executed = true;
            }
        }

        if (executed)
        {
            if (isConsumable)
            {
                Debug.Log(itemName + " を消費した");
                InventoryManager.Instance.RemoveItemByID(itemID);
            }

            // 実際に効果が発動したときだけ閉じる
            PauseMenu.Instance.Resume();
        }
        else
        {
            Debug.Log(itemName + " は今使えない！");
            // ここでは閉じずに残すか、閉じるかは仕様次第
        }
    }
}