using UnityEngine;

public class GateController : MonoBehaviour
{
    // �C���X�y�N�^�[����\��������Gate�Q�[���I�u�W�F�N�g�����蓖�Ă�
    public GameObject gateObject;

    void OnTriggerEnter2D(Collider2D other)
    {
        // �v���C���[���g���K�[�ɓ��������Ƃ��m�F
        if (other.CompareTag("Player"))
        {
            // �v���C���[���g���K�[���ɂ���ԁAEnter�L�[�̓��͂��`�F�b�N
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Gate�Q�[���I�u�W�F�N�g��L����
                if (gateObject != null)
                {
                    gateObject.SetActive(true);
                }
            }
        }
    }
}