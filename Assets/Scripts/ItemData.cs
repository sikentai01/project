using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("��{���")]
    public string itemID;           // �� ID ��ǉ�
    public string itemName;
    [TextArea] public string description;
    public bool isConsumable;

    [Header("���ʃ��X�g (ScriptableObject �Q��)")]
    [SerializeField] private List<ItemEffect> effects = new List<ItemEffect>();
    public List<ItemEffect> Effects => effects;

    /// <summary>
    /// �A�C�e�����g�p�����Ƃ��̏���
    /// </summary>
    public void Use()
    {
        if (effects == null || effects.Count == 0)
        {
            Debug.Log(itemName + " �͌��ʂ��ݒ肳��Ă��Ȃ�");
            return;
        }

        foreach (var effect in effects)
        {
            effect.Execute(this); // ������n���Ď��s
        }

        if (isConsumable)
        {
            Debug.Log(itemName + " ��������I");
            InventoryManager.Instance.RemoveItemByID(itemID);
        }
    }
}