using UnityEngine;
using UnityEngine.SceneManagement;

public class FallTrap : MonoBehaviour
{
    private Collider2D trapCollider;

    void Start()
    {
        trapCollider = GetComponent<Collider2D>();

        // �����Z�[�u�ς݂Ȃ�A�ŏ�����g���K�[OFF�ɂ��ĕǉ�
        if (GameFlags.SaveTriggered)
        {
            DisableTrap();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �g���K�[����Ȃ����͉������Ȃ�
        if (!trapCollider.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("�v���C���[�����Ƃ����ɗ������I");
            // �����ŃQ�[���I�[�o�[����
            SceneManager.LoadScene("GameOver");
        }
    }

    // ====== �����ŗ��Ƃ������u�������v ======
    public void DisableTrap()
    {
        if (trapCollider != null)
        {
            trapCollider.isTrigger = false; // �� ����ŕǂɂ���
            Debug.Log("���Ƃ����𖳌��� �� �ǂɂ��܂���");
        }
    }
}
//���܂������Ă�H