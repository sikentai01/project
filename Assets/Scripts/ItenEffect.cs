using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    // 使用できるかどうか（デフォルトは常に使える）
    public virtual bool CanExecute(ItemData item) => true;

    // 実際の効果
    public abstract void Execute(ItemData item);
}