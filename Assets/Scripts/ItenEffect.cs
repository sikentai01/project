using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    // �g�p�ł��邩�ǂ����i�f�t�H���g�͏�Ɏg����j
    public virtual bool CanExecute(ItemData item) => true;

    // ���ۂ̌���
    public abstract void Execute(ItemData item);
}