using UnityEngine;

public class SyncTrigger : MonoBehaviour
{
    // �v���C���[���g���K�[���ɂ��邩�ǂ����̃t���O
    public bool isPlayerInside = false;

    // �����ɑ���SyncTrigger�̃��W�b�N��ǉ�

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}