using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, ISelectHandler
{
    public TMP_Text slotText;   // �X���b�g���ɕ\������e�L�X�g

    private ItemData currentItem;  // �� InventoryItem �ł͂Ȃ� ItemData �ɕύX
    private InventoryUI inventoryUI;

    void Start()
    {
        // �V�[�����ɂ��� InventoryUI ��T��
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        // ���̃X���b�g���{�^���Ȃ�A�N���b�N���ɌĂ΂��悤�ɐݒ�
        GetComponent<Button>().onClick.AddListener(OnClickSlot);
    }

    // �A�C�e�����Z�b�g���ĕ\��
    public void SetItem(ItemData item)
    {
        currentItem = item;
        slotText.text = item.itemName; // ���O��\��
    }

    // �A�C�e�����Ȃ��ꍇ�̓N���A
    public void ClearSlot()
    {
        currentItem = null;
        slotText.text = "";
    }

    // ���̃X���b�g��I�񂾂Ƃ��ɐ��������X�V
    public void OnSelectSlot()
    {
        if (currentItem != null)
            inventoryUI.ShowDescription(currentItem);
        else
            inventoryUI.ShowDescription(null);
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelectSlot();
    }

    // Enter�i�N���b�N�j�Ŏg�p
    // Enter�i�N���b�N�j�Ŏg�p
    public void OnClickSlot()
    {
        if (currentItem == null) return;

        // �V�[�����̂��ׂĂ� ItemTrigger ���擾
        var triggers = Object.FindObjectsByType<ItemTrigger>(
            FindObjectsSortMode.None
        );

        foreach (var trigger in triggers)
        {
            // �v���C���[���߂��A�����̃A�C�e�����L���Ȃ�g��
            if (trigger.IsPlayerNear && trigger.HasPendingGimmick(currentItem))
            {
                Debug.Log($"[ItemSlot] {currentItem.itemName} �� {trigger.name} �Ɏg�p");
                trigger.UseItemOnGimmick(currentItem);
                PauseMenu.Instance.Resume();
                return;
            }
        }

        // �ǂ̃g���K�[�ɂ����Ă͂܂�Ȃ�������ʏ�g�p
        Debug.Log($"[ItemSlot] {currentItem.itemName} ��ʏ�g�p�I");
        currentItem.Use();

        PauseMenu.Instance.Resume();
        Debug.Log("[ItemSlot] OnClickSlot �I��");
    }
}