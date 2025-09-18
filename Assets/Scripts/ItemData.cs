using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    [Header("��{���")]
    public string itemID;
    public string itemName;
    [TextArea] public string description;
    public bool isConsumable;

    [Header("�f�o�b�O�p")]
    public bool DebugOwned;   // �� �����ǉ�

    [Header("���ʃ��X�g")]
    [SerializeField] private List<ItemEffect> effects = new List<ItemEffect>();
    public List<ItemEffect> Effects => effects;

    public void Use()
    {
        if (effects == null || effects.Count == 0)
        {
            Debug.Log(itemName + " �͌��ʂ��ݒ肳��Ă��Ȃ�");
            return;
        }

        bool executed = false;

        foreach (var effect in effects)
        {
            if (effect.CanExecute(this))  // �� �������A�C�e���̌��ʂɈς˂�
            {
                effect.Execute(this);      // ���s
                executed = true;
            }
        }

        if (executed)
        {
            if (isConsumable)
            {
                Debug.Log(itemName + " �������");
                InventoryManager.Instance.RemoveItemByID(itemID);
            }

            // ���ۂɌ��ʂ����������Ƃ���������
            PauseMenu.Instance.Resume();
        }
        else
        {
            Debug.Log(itemName + " �͍��g���Ȃ��I");
            // �����ł͕����Ɏc�����A���邩�͎d�l����
        }
    }
}