using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    // �g�p�\����i�K�v�Ȃ�I�[�o�[���C�h�j
    public virtual bool CanExecute(ItemData item) => true;

    // ���s�i�e�G�t�F�N�g���Ƃɋ�̉��j
    public abstract void Execute(ItemData item);
}