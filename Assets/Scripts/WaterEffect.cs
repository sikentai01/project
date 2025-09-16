using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effects/WaterEffect")]
public class WaterEffect : ItemEffect
{
    public override void Execute(ItemData item)
    {
        Debug.Log(item.itemName + " ���g���Đ����܂����I");
        // �����Ɍ��ʂ�����
        if (item.isConsumable)
        {
            InventoryManager.Instance.RemoveItemByID(item.name);
            // item.name ����Ȃ��āA�ʂ� itemID �v���p�e�B��ǉ����Ă�����
        }
    }
}