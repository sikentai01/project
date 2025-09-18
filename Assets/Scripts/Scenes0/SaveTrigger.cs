using UnityEngine;
using System.Collections;

public class SaveTrigger : MonoBehaviour
{
    private bool isPlayerNear = false;
    private GridMovement player;

    [Header("�K�v�ȕ��� (0=��, 1=��, 2=�E, 3=��, -1=�����Ȃ�)")]
    public int requiredDirection = 3; // �f�t�H���g: �����

    [Header("��b�C�x���g�œn���A�C�e��")]
    public ItemData rewardItem;   // Inspector�Őݒ肷��

    void Start()
    {
        player = FindFirstObjectByType<GridMovement>();
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            if (requiredDirection == -1 || player.GetDirection() == requiredDirection)
            {
                StartCoroutine(EventFlow());
            }
            else
            {
                Debug.Log("�������Ⴄ�̂Œ��ׂ��Ȃ�");
            }
        }
    }

    private IEnumerator EventFlow()
    {
        Debug.Log("�Z�[�u���ׂ�");

        // 1. �v���C���[�̓������~�߂� & ���j���[�֎~
        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;

        // ���������ɌŒ�
        if (player != null) player.SetDirection(0);

        yield return new WaitForSeconds(1.5f);

        // 2. ��b���O�i�L�����o���j
        Debug.Log("�L���������ꂽ: �w�悭�����ȁx");

        yield return new WaitForSeconds(3f);

        // 3. �A�C�e������
        if (rewardItem != null)
        {
            InventoryManager.Instance.AddItem(rewardItem);
            Debug.Log($"�L��������A�C�e���w{rewardItem.itemName}�x���󂯎�����I");
        }
        else
        {
            Debug.LogWarning("rewardItem ���ݒ肳��Ă��܂���");
        }

        yield return new WaitForSeconds(3f);

        // 4. ��b�I��
        Debug.Log("�L����: �w�ł͂܂�����c�x");

        yield return new WaitForSeconds(2f);

        // 5. �v���C���[�̓�����߂� & ���j���[����
        if (player != null) player.enabled = true;
        PauseMenu.blockMenu = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}