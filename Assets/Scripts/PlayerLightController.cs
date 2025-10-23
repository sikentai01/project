using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightController : MonoBehaviour
{
    [Header("���C�g�ݒ�")]
    [Tooltip("�����d�����C�g�i�v���C���[�{�̂ɕt���Ă� Light2D�j")]
    public Light2D normalLight;      // �� Player�{�̂ɃA�^�b�`����Ă郉�C�g

    [Tooltip("�������C�g�i�v���C���[�̎q�I�u�W�F�N�g���j")]
    public Light2D restrictedLight;  // �� Player�̎q�I�u�W�F�N�g�ɂ��郉�C�g

    [Header("�A�C�e������")]
    [Tooltip("�����d���A�C�e����ID�iInventoryManager�ɓo�^����Ă���itemID�j")]
    public string flashlightItemID = "item_flashlight";

    private InventoryManager inventory;

    private void Start()
    {
        inventory = InventoryManager.Instance;
        UpdateLightState(); // ������ԃ`�F�b�N
    }

    private void Update()
    {
        // �����`�F�b�N�i�K�v�Ȃ�C�x���g�ɒu�������j
        UpdateLightState();
    }

    /// <summary>
    /// �����d���A�C�e���̏����󋵂ɉ����ă��C�g��؂�ւ���
    /// </summary>
    public void UpdateLightState()
    {
        if (inventory == null)
        {
            Debug.LogWarning("[PlayerLightController] InventoryManager ��������܂���");
            return;
        }

        bool hasFlashlight = inventory.HasItem(flashlightItemID);

        if (hasFlashlight)
        {
            //  �����d������ �� �ʏ탉�C�gON�A�������C�gOFF
            SetNormalLight(true);
        }
        else
        {
            //  �����d���Ȃ� �� �ʏ탉�C�gOFF�A�������C�gON
            SetNormalLight(false);
        }
    }

    /// <summary>
    /// �����I�Ƀ��C�g��؂�ւ���i���X�N���v�g����Ăяo���j
    /// </summary>
    public void SetNormalLight(bool enabled)
    {
        if (normalLight != null) normalLight.enabled = enabled;
        if (restrictedLight != null) restrictedLight.enabled = !enabled;
    }

    /// <summary>
    /// �����d��������ON�ɂ���
    /// </summary>
    public void ForceFlashlightOn()
    {
        SetNormalLight(true);
    }

    /// <summary>
    /// �����d��������OFF�ɂ���
    /// </summary>
    public void ForceFlashlightOff()
    {
        SetNormalLight(false);
    }
}