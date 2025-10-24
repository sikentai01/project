using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effects/Poisoning Effect", fileName = "PoisoningEffect")]
public class PoisoningEffect : ItemEffect
{
    // [Header("�g�p��ɃA�N�e�B�u�ɂ���GameOver�R���g���[���[")]
    // public GameObject gameOverControllerObject; // �� ���̃t�B�[���h�͕s�v�ɂȂ�܂�

    [Header("�g�p���ɃC���x���g������폜���邩")]
    public bool isConsumable = true;

    /// <summary>
    /// �A�C�e���g�p�̏����i�펞�g�p�\�Ƃ���j
    /// </summary>
    public override bool CanExecute(ItemData item)
    {
        // �ŕr�͂ǂ��ł��g���邪�A���ʂƂ��ăQ�[���I�[�o�[�ɂȂ�Ƃ����ݒ�
        return true;
    }

    /// <summary>
    /// ���s�F�Q�[���I�[�o�[���N�����A�A�C�e�����폜����
    /// </summary>
    public override void Execute(ItemData usedItem)
    {
        // 1. �A�C�e�����C���x���g������폜
        if (isConsumable && InventoryManager.Instance != null)
        {
            // ItemData�ɂ� itemID ������Ƒz�肵�A������g���č폜����
            InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            Debug.Log($"[PoisoningEffect] {usedItem.itemName} ���g�p���A�폜���܂����B");
        }

        // 2. �Q�[���I�[�o�[�����̎��s (BootLoader�ɂ��V�[���؂�ւ�)
        if (BootLoader.Instance != null)
        {
            // BootLoader�o�R��GameOver�V�[���ɑ����ɐ؂�ւ���
            BootLoader.Instance.SwitchSceneInstant("GameOver");
            Debug.Log("[PoisoningEffect] �ŕr���g�p �� GameOver�V�[���֑����؂�ւ�");
        }
        else
        {
            // BootLoader���Ȃ��ꍇ�́A�]���̏����̑�փ��O���o��
            Debug.LogError("[PoisoningEffect] BootLoader.Instance��������܂���BGameOver�������X�L�b�v���܂����B");
        }
    }
}