using UnityEngine;

public class DummyGimmick : GimmickBase
{
    public ItemData requiredItem;  // �K�v�ȃA�C�e���i��F���j

    // ItemSlot����Ă΂��p�̃��\�b�h
    public void UseItemForGimmick(ItemData usedItem, ItemTrigger trigger)
    {
        if (usedItem == requiredItem)
        {
            Debug.Log(requiredItem.itemName + " ���g���Ďd�|�����������I");
            InventoryManager.Instance.RemoveItemByID(requiredItem.itemID);

            // �M�~�b�N����
            Complete(trigger);
        }
        else
        {
            Debug.Log("���̃M�~�b�N�ł͎g���Ȃ��A�C�e���ł�");
        }
    }

    // ����Enter�ŌĂ΂��̂͂�������
    public override void StartGimmick(ItemTrigger trigger)
    {
        Debug.Log("�A�C�e���X���b�g����g�p���Ă�������");
    }
}