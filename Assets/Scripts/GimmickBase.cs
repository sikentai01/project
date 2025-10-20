using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    // デフォルトは「アイテム不要」
    public virtual bool NeedsItem => false;

    public virtual bool CanUseItem(ItemData item) { return false; }

    public virtual void UseItem(ItemData item) { }

    public abstract void StartGimmick();
}