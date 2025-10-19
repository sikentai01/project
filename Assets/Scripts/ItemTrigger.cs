using System.Collections.Generic;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [Header("���̃g���K�[�ŗL��ID�i�Z�[�u�p�Ƀ��j�[�N�ɐݒ�j")]
    public string triggerID;

    [Header("�E����A�C�e���f�[�^")]
    public ItemData itemData;

    [Header("�����ڂ������I�u�W�F�N�g�i���⏼���Ȃǁj")]
    public GameObject targetObject;

    [Header("�K�v�Ȍ��� (0=��,1=��,2=�E,3=��, -1=�����Ȃ�)")]
    public int requiredDirection = -1;

    [Header("�M�~�b�N (�C��)")]
    public List<GimmickBase> gimmicks = new List<GimmickBase>();

    [Header("���݂̐i�s�x (�Z�[�u�Ώ�)")]
    public int currentStage = 0;

    private bool isPlayerNear = false;
    private GridMovement playerMovement;

    public bool IsPlayerNear => isPlayerNear;

    void Update()
    {
        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // �����`�F�b�N
            if (requiredDirection != -1 && playerMovement != null && playerMovement.GetDirection() != requiredDirection)
            {
                Debug.Log("�����������Ă��Ȃ��̂ŃA�C�e�����擾�ł��Ȃ�");
                return;
            }

            // �M�~�b�N���c���Ă���ꍇ
            if (currentStage < gimmicks.Count)
            {
                Debug.Log($"�M�~�b�N {currentStage} �J�n");
                gimmicks[currentStage].StartGimmick(this);
            }
            else
            {
                CollectItem();
            }
        }
    }

    // �M�~�b�N������
    public void CompleteCurrentGimmick()
    {
        currentStage++;
        Debug.Log($"�i�s�i�K�� {currentStage} �ɂȂ���");

        if (currentStage >= gimmicks.Count)
        {
            CollectItem();
        }
    }

    // �A�C�e������
    public void CollectItem()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            Debug.Log(itemData.itemName + " �����I");

            if (targetObject != null)
                targetObject.SetActive(false);
            else
                gameObject.SetActive(false);
        }
    }

    //  ���̃g���K�[���u�A�C�e���g�p�ɑΉ����Ă��邩�v�𔻒�
    public bool HasPendingGimmick(ItemData item)
    {
        if (currentStage < gimmicks.Count)
        {
            var gimmick = gimmicks[currentStage];
            Debug.Log($"[HasPendingGimmick] {gimmick.name} �� {item.itemName} ���m�F��");

            if (!gimmick.NeedsItem) return false;
            bool result = gimmick.CanUseItem(item);
            Debug.Log($"[HasPendingGimmick] ���茋�� = {result}");
            return result;
        }
        return false;
    }

    //  �A�C�e�����M�~�b�N�Ɏg�p
    public void UseItemOnGimmick(ItemData item)
    {
        if (!isPlayerNear) return;

        if (requiredDirection != -1 && playerMovement != null && playerMovement.GetDirection() != requiredDirection)
            return;

        if (currentStage < gimmicks.Count)
        {
            var gimmick = gimmicks[currentStage];

            if (gimmick.NeedsItem)
            {
                if (gimmick.CanUseItem(item))
                {
                    gimmick.UseItem(item, this);
                }
                else
                {
                    Debug.Log("���̃A�C�e���͎g���܂���");
                }
            }
            else
            {
                gimmick.StartGimmick(this); // �A�C�e���s�v�M�~�b�N
            }
        }
        else
        {
            CollectItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            playerMovement = other.GetComponent<GridMovement>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            playerMovement = null;
        }
    }
}
