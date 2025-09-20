using UnityEngine;

public class GiveKeyGimmick : GimmickBase
{
    [Header("�K�v�ȃA�C�e��")]
    public ItemData requiredItem;

    public override bool NeedsItem => true;

    // ���̃M�~�b�N���A�C�e�����󂯕t���邩����
    public override bool CanUseItem(ItemData item)
    {
        return item == requiredItem;
    }

    // �A�C�e���g�p����
    public override void UseItem(ItemData usedItem, ItemTrigger trigger)
    {
        if (usedItem == requiredItem)
        {
            Debug.Log(requiredItem.itemName + " ���g���Ĕ����J�����I");
            InventoryManager.Instance.RemoveItemByID(requiredItem.itemID);

            // �M�~�b�N����
            Complete(trigger);
        }
        else
        {
            Debug.Log("���̃M�~�b�N�ł͎g���Ȃ��A�C�e���ł�");
        }
    }

    // �M�~�b�N�J�n�iEnter�������͖����j
    public override void StartGimmick(ItemTrigger trigger)
    {
        Debug.Log("�A�C�e���X���b�g����g�p���Ă�������");
    }
}