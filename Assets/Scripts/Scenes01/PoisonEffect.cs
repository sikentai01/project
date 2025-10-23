using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effects/Poisoning Effect", fileName = "PoisoningEffect")]
public class PoisoningEffect : ItemEffect
{
    [Header("�g�p��ɃA�N�e�B�u�ɂ���GameOver�R���g���[���[")]
    public GameObject gameOverControllerObject;

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

        // 2. �Q�[���I�[�o�[�����̎��s
        if (gameOverControllerObject != null)
        {
            // GameOverController.cs���A�^�b�`���ꂽ�Q�[���I�u�W�F�N�g���A�N�e�B�u�ɂ���
            gameOverControllerObject.SetActive(true);
            Debug.Log("[PoisoningEffect] �ŕr���g�p �� �Q�[���I�[�o�[���N��");
        }
        else
        {
            Debug.LogError("[PoisoningEffect] GameOverController�I�u�W�F�N�g���ݒ肳��Ă��܂���B");
        }
    }
}