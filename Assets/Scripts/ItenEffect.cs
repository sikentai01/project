using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    // 使用可能判定（必要ならオーバーライド）
    public virtual bool CanExecute(ItemData item) => true;

    // 実行（各エフェクトごとに具体化）
    public abstract void Execute(ItemData item);
}