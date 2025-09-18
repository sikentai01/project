using UnityEngine;

[System.Serializable]
public class GiveKeyGimmick : GimmickBase
{
    [Header("�g�p����A�C�e��ID�iKeyBand��ID�j")]
    public string requiredItemID;

    [Header("���肷��A�C�e���f�[�^")]
    public ItemData itemToGive;

    // �M�~�b�N�̊J�n���Ɏ��s�����
    public override void StartGimmick(ItemTrigger trigger)
    {
        // �K�v�ȃA�C�e���������Ă��邩�`�F�b�N
        if (InventoryManager.Instance.HasItem(requiredItemID))
        {
            // �A�C�e��������ē���
            InventoryManager.Instance.RemoveItemByID(requiredItemID);
            InventoryManager.Instance.AddItem(itemToGive);
            Debug.Log(itemToGive.itemName + " ����肵�܂����I");

            // �M�~�b�N�������������Ƃ��g���K�[�ɓ`����
            trigger.CompleteCurrentGimmick();
        }
        else
        {
            Debug.Log("�A�C�e��������܂���B");
        }
    }
}
