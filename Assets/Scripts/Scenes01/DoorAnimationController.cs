using UnityEngine;
using System.Collections;

public class DoorAnimationController : MonoBehaviour
{
    // �h�A���g��Animator�����蓖�Ă�
    public Animator doorAnimator;

    // �v���C���[�̈ړ��X�N���v�g�ւ̎Q��
    private MonoBehaviour playerMovementScript;

    private bool playerIsNearDoor = false;
    private bool isAnimationPlaying = false;

    // Start�̓Q�[���J�n���Ɉ�x�����Ă΂�܂�
    void Start()
    {
        // �V�[���Ɋ֌W�Ȃ��AGridMovement�X�N���v�g�������ŒT���Ċ��蓖�Ă�
        playerMovementScript = FindObjectOfType<GridMovement>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearDoor = true;
            Debug.Log("�v���C���[���h�A�ɋ߂Â���");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearDoor = false;
            Debug.Log("�v���C���[���h�A���痣�ꂽ");
        }
    }

    void Update()
    {
        if (playerIsNearDoor && !isAnimationPlaying && Input.GetKeyDown(KeyCode.Return))
        {
            isAnimationPlaying = true;

            // �v���C���[�̈ړ��X�N���v�g���ꎞ�I�ɖ�����
            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = false;
            }

            // �h�A�̃A�j���[�V�������Đ�
            doorAnimator.Play("OpenGate", 0);

            // �A�j���[�V�����I����Ɉړ����ĊJ������R���[�`�����J�n
            StartCoroutine(WaitForAnimationEnd());
        }
    }

    private IEnumerator WaitForAnimationEnd()
    {
        yield return null;

        // �Đ����̃A�j���[�V�����̒������擾���đҋ@
        yield return new WaitForSeconds(doorAnimator.GetCurrentAnimatorStateInfo(0).length);

        // �v���C���[�̈ړ��X�N���v�g��L����
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
        isAnimationPlaying = false;
    }
}