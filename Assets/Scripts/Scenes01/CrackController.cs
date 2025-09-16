using UnityEngine;

public class CrackController : MonoBehaviour
{
    public GameObject crackEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (crackEffect != null)
            {
                // �q�r�̃Q�[���I�u�W�F�N�g��L����
                crackEffect.SetActive(true);
            }

            // ���̃g���K�[�͈�x�����g��Ȃ����߁A������
            gameObject.SetActive(false);
        }
    }
}