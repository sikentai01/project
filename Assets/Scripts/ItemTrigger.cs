using UnityEngine;
using System.Collections.Generic;

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

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // �����`�F�b�N
            if (requiredDirection != -1 && playerMovement != null && playerMovement.GetDirection() != requiredDirection)
            {
                Debug.Log("�����������Ă��Ȃ��̂ŃA�C�e�����擾�ł��Ȃ�");
                return;
            }

            // �M�~�b�N���c���Ă���Ȃ炻���������
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

    // �M�~�b�N�������󂯎��
    public void CompleteCurrentGimmick()
    {
        currentStage++;
        Debug.Log($"�i�s�i�K�� {currentStage} �ɂȂ���");

        // �S���I�������A�C�e�������
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
                gameObject.SetActive(false); // Destroy ����Ȃ���\��
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