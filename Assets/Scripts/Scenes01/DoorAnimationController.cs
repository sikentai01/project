using UnityEngine;
using System.Collections;

public class DoorAnimationController : MonoBehaviour
{
    public Animator characterAnimator;
    public MonoBehaviour movementScript;

    private bool isAnimationPlaying = false;

    // �g���K�[�R���C�_�[�ɓ������Ƃ��Ɉ�x�����Ăяo�����
    void OnTriggerEnter2D(Collider2D other)
    {
         Debug.Log("�h�A�ɋ߂Â��܂���"); // �f�o�b�O�p
    }

    // �g���K�[�R���C�_�[�̒��ɂ���ԁA���t���[���Ăяo�����
    void OnTriggerStay2D(Collider2D other)
    {
        // �A�j���[�V�����Đ����łȂ��A����Enter�L�[�������ꂽ�ꍇ
        if (!isAnimationPlaying && other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Return))
        {
            isAnimationPlaying = true;

            // �ړ��X�N���v�g���ꎞ�I�ɖ���������
            if (movementScript != null)
            {
                movementScript.enabled = false;
            }

            // �A�j���[�V�������Đ�
            characterAnimator.Play("OpenGate");

            // �A�j���[�V�������I������܂őҋ@����R���[�`�����J�n
            StartCoroutine(WaitForAnimationEnd());
        }
    }

    // �g���K�[�R���C�_�[����o���Ƃ��ɌĂяo�����
    void OnTriggerExit2D(Collider2D other)
    {
        // Debug.Log("�h�A���痣��܂���"); // �f�o�b�O�p
    }

    private IEnumerator WaitForAnimationEnd()
    {
        yield return null;

        yield return new WaitForSeconds(characterAnimator.GetCurrentAnimatorStateInfo(0).length);

        if (movementScript != null)
        {
            movementScript.enabled = true;
        }
        isAnimationPlaying = false;
    }
}