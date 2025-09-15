using UnityEngine;

public class CrackController : MonoBehaviour
{
    // �C���X�y�N�^�[����q�r�̃Q�[���I�u�W�F�N�g�����蓖�Ă�
    public GameObject crackEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �ڐG�����I�u�W�F�N�g��Yuki�i�v���C���[�j�ł��邱�Ƃ��m�F
        if (other.CompareTag("Player"))
        {
            // �q�r�̃Q�[���I�u�W�F�N�g��L�������ĕ\��
            if (crackEffect != null)
            {
                crackEffect.SetActive(true);
            }

            // ���̃g���K�[�͈�x�����g��Ȃ����߁A����������
            gameObject.SetActive(false);
        }
    }
}