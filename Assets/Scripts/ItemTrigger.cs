using UnityEngine;
using System.Collections.Generic;

public class ItemTrigger : MonoBehaviour
{
    [Header("�E����A�C�e���f�[�^")]
    public ItemData itemData;   // ScriptableObject ���A�^�b�`����
    [Header("�����ڂ������I�u�W�F�N�g�i���⏼���Ȃǁj")]
    public GameObject targetObject;
    [Header("�K�v�Ȍ��� (0=��,1=��,2=�E,3=��, -1=�����Ȃ�)")]
    public int requiredDirection = -1; // -1�Ȃ�ǂ̌����ł�OK

    [Header("�M�~�b�N (�C��)")]
    public List<GimmickBase> gimmicks = new List<GimmickBase>();

    private int currentGimmickIndex = 0;
    private bool isPlayerNear = false;
    private GridMovement playerMovement;

    // �� �M�~�b�N���I�������u�ҋ@���v���ǂ���
    private bool waitingForNext = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return)) // Enter�Ői�s
        {
            // �����`�F�b�N
            if (requiredDirection != -1 && playerMovement != null && playerMovement.GetDirection() != requiredDirection)
            {
                Debug.Log("�����������Ă��Ȃ��̂ŃA�C�e�����擾�ł��Ȃ�");
                return;
            }

            if (waitingForNext)
            {
                // ���ɐi�ޏ���
                waitingForNext = false;
                ContinueGimmickOrItem();
                return;
            }

            // �M�~�b�N�J�n
            if (currentGimmickIndex < gimmicks.Count)
            {
                gimmicks[currentGimmickIndex].StartGimmick(this);
            }
            else
            {
                CollectItem();
            }
        }
    }

    public void CompleteCurrentGimmick()
    {
        Debug.Log("�M�~�b�N�����I���̓��͂�҂��Ă��܂�...");
        waitingForNext = true; // �� ����Enter��҂�
    }

    private void ContinueGimmickOrItem()
    {
        currentGimmickIndex++;
        if (currentGimmickIndex < gimmicks.Count)
        {
            gimmicks[currentGimmickIndex].StartGimmick(this);
        }
        else
        {
            CollectItem();
        }
    }

    public void CollectItem()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            Debug.Log(itemData.itemName + " �����I");

            if (targetObject != null)
                targetObject.SetActive(false);
            else
                Destroy(gameObject);
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