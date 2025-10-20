using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    // �f�t�H���g�́u�A�C�e���s�v�v
    public virtual bool NeedsItem => false;

    public virtual bool CanUseItem(ItemData item) { return false; }

    public virtual void UseItem(ItemData item) { }

    public abstract void StartGimmick();
}