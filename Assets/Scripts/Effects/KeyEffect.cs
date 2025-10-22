using UnityEngine;

/// <summary>
/// ���A�C�e���̌���
/// �v���C���[���߂��̃h�A�̑O�ɗ����Ă���Ƃ��̂ݎg�p�ł���B
/// �����E��ID����v���Ă���ΊJ���B����ȊO�ł͎g�p�s�B
/// </summary>
[CreateAssetMenu(menuName = "Game/Effects/KeyEffect")]
public class KeyEffect : ItemEffect
{
    public override bool CanExecute(ItemData item)
    {
        //  �h�A���߂��ɂ���A�����ƌ�ID����v���Ă���ꍇ�̂� true
        var door = DoorController.PlayerNearbyDoor;
        if (door == null) return false;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        var move = player.GetComponent<GridMovement>();
        if (move == null) return false;

        // ��������v���Ă��Ȃ���Ύg���Ȃ�
        if (move.GetDirection() != door.RequiredDirection)
            return false;

        // �����K�v�Ȃ��h�A�͑ΏۊO
        string requiredKey = door.GetRequiredKeyID();
        if (string.IsNullOrEmpty(requiredKey)) return false;

        // ��ID����v���Ă���Ƃ��̂ݎg�p�\
        return requiredKey == item.itemID && door.currentStage == 0;
    }

    public override void Execute(ItemData item)
    {
        var door = DoorController.PlayerNearbyDoor;
        if (door == null)
        {
            Debug.Log("[KeyEffect] �h�A�̑O�ɂ��Ȃ����ߎg�p�ł��܂���B");
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var move = player.GetComponent<GridMovement>();
        if (move == null) return;

        // ��������v���Ă��Ȃ���Ζ���
        if (move.GetDirection() != door.RequiredDirection)
        {
            Debug.Log("[KeyEffect] �������Ⴄ���ߌ����g���܂���B");
            return;
        }

        // �K�v�Ȍ�����v���Ă��邩�m�F
        string requiredKey = door.GetRequiredKeyID();
        if (requiredKey != item.itemID)
        {
            Debug.Log("[KeyEffect] ���̌��͂��̃h�A�ɍ����܂���B");
            return;
        }

        // �����ς݂Ȃ�X�L�b�v
        if (door.currentStage == 1)
        {
            Debug.Log("[KeyEffect] ���łɊJ���Ă��܂��B");
            return;
        }

        //  �������s
        bool success = door.TryUseKey(item.itemID);
        if (success)
        {
            Debug.Log($"[KeyEffect] {item.itemName} ���g���� {door.name} ���J�����I");

            if (item.isConsumable)
                InventoryManager.Instance.RemoveItemByID(item.itemID);
        }
    }
}