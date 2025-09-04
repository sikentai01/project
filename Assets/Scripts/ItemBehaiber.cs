using UnityEngine;
using System.Collections.Generic;

public class ItemBehaviour : MonoBehaviour
{
    public string itemID;
    public string itemName;
    [TextArea] public string description;

    [Header("���ʃ��X�g�i�����A�^�b�`�j")]
    public List<ItemEffect> effects = new List<ItemEffect>();

    public void Use()
    {
        if (effects.Count == 0)
        {
            Debug.Log(itemName + " �͌��ʂ��ݒ肳��Ă��Ȃ�");
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