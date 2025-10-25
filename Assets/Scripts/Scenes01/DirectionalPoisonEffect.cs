using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effects/Directional Poison", fileName = "DirectionalPoisonEffect")]
public class DirectionalPoisonEffect : ItemEffect
{
    [Header("�����ɕK�v�Ȍ��� (0=��, 1=��, 2=�E, 3=��, -1=�����Ȃ�)")]
    public int requiredDirection = -1;

    [Header("�ΏۂƂȂ�GimmickTrigger��ID (�󗓂ŋ߂��̂ǂ�GimmickTrigger�ł���)")]
    public string targetTriggerID;

    [Header("�g�p���ɃC���x���g������폜���邩")]
    public bool isConsumable = true;

    /// <summary>
    /// �A�C�e���g�p�̏����F����̃R���C�_�[���ɂ��āA����̕����������Ă��邩�`�F�b�N����
    /// </summary>
    public override bool CanExecute(ItemData item)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        // 1. �����̃`�F�b�N
        if (requiredDirection != -1)
        {
            var movement = player.GetComponent<GridMovement>();
            if (movement == null) return false;

            if (movement.GetDirection() != requiredDirection)
            {
                // �������Ⴄ���ߎg�p�s��
                return false;
            }
        }

        // 2. �R���C�_�[�͈͂̃`�F�b�N
        var triggers = Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        bool isInValidArea = false;

        foreach (var trigger in triggers)
        {
            if (!trigger.IsPlayerNear) continue; // �v���C���[���͈͊O

            // �^�[�Q�b�gID���ݒ肳��Ă���ꍇ��ID���`�F�b�N
            if (!string.IsNullOrEmpty(targetTriggerID) && trigger.gimmickID != targetTriggerID)
            {
                continue; // ID����v���Ȃ����ߖ���
            }

            // �v���C���[���͈͓��ɂ��āAID����v�����i�܂���ID���ݒ肳��Ă��Ȃ��j
            isInValidArea = true;
            break;
        }

        if (!isInValidArea)
        {
            Debug.LogWarning("[DirectionalPoisonEffect] �v���C���[�͑Ώۂ̃g���K�[�͈͊O�ł��B");
            return false;
        }

        // �����`�F�b�N�ƃR���C�_�[�`�F�b�N�̗������N���A
        return true;
    }

    /// <summary>
    /// ���s�F�Q�[���I�[�o�[���N�����A�A�C�e�����폜����
    /// </summary>
    public override void Execute(ItemData usedItem)
    {
        // Execute���Ă΂ꂽ���_�ł́ACanExecute()��true��Ԃ��Ă��邱�Ƃ��ۏ؂���Ă���

        // 1. �A�C�e�����C���x���g������폜
        if (isConsumable && InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            Debug.Log($"[DirectionalPoisonEffect] {usedItem.itemName} ���g�p���A�폜���܂����B");
        }

        // 2. �Q�[���I�[�o�[�����̎��s (BootLoader�ɂ��V�[���؂�ւ�)
        if (BootLoader.Instance != null)
        {
            BootLoader.Instance.SwitchSceneInstant("GameOver");
            Debug.Log("[DirectionalPoisonEffect] �ŕr���g�p �� GameOver�V�[���֑����؂�ւ�");
        }
        else
        {
            Debug.LogError("[DirectionalPoisonEffect] BootLoader.Instance��������܂���BGameOver�������X�L�b�v���܂����B");
        }
    }
}