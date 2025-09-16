using UnityEngine;
using System.Collections;

public class GateTriggerController : MonoBehaviour
{
    // �C���X�y�N�^�[����\��������Gate�Q�[���I�u�W�F�N�g�����蓖�Ă�
    public GameObject gateObject;

    // �g���K�[�̒��ɂ���ԁA���t���[���Ăяo�����
    void OnTriggerStay2D(Collider2D other)
    {
        // �v���C���[��"Player"�^�O�����Ă��邱�Ƃ��m�F
        if (other.CompareTag("Player"))
        {
            // Enter�L�[�������ꂽ���`�F�b�N
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Gate�Q�[���I�u�W�F�N�g��L�����i�\���j
                if (gateObject != null)
                {
                    gateObject.SetActive(true);
                }

                // 1�b��ɔ�\���ɂ���R���[�`�����J�n
                StartCoroutine(WaitAndHide());
            }
        }
    }

    private IEnumerator WaitAndHide()
    {
        // 1�b�ԑҋ@
        yield return new WaitForSeconds(1.0f);

        // Gate�Q�[���I�u�W�F�N�g�𖳌����i��\���j
        if (gateObject != null)
        {
            gateObject.SetActive(false);
        }
    }
}