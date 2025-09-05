using UnityEngine;

public class DoorAnimationController : MonoBehaviour
{
    // �C���X�y�N�^�[����L�����N�^�[��Animator�����蓖�Ă�
    public Animator characterAnimator;

    // �v���C���[���R���C�_�[���ɂ��邩�ǂ����̃t���O
    private bool playerIsNearDoor = false;

    // �g���K�[�R���C�_�[�ɓ������Ƃ��ɌĂяo�����
    void OnTriggerEnter2D(Collider2D other)
    {
        // �v���C���[��"Player"�^�O�����Ă��邩�m�F
        if (other.CompareTag("Player"))
        {
            playerIsNearDoor = true;
            Debug.Log("�v���C���[���h�A�̋߂��ɓ���܂���"); // �������ǉ�
        }
    }

    // �g���K�[�R���C�_�[����o���Ƃ��ɌĂяo�����
    void OnTriggerExit2D(Collider2D other)
    {
        // �v���C���[��"Player"�^�O�����Ă��邩�m�F
        if (other.CompareTag("Player"))
        {
            playerIsNearDoor = false;
            Debug.Log("�v���C���[���h�A���痣��܂���"); // �������ǉ�
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // �v���C���[���h�A�̋߂��ɂ��āA�G���^�[�L�[�������ꂽ���`�F�b�N
        if (playerIsNearDoor && Input.GetKeyDown(KeyCode.Return))
        {
            // Animator�Ƀg���K�[���Z�b�g���āA�A�j���[�V�������Đ�
            characterAnimator.SetTrigger("OpenGateTrigger");
        }
    }
}
