using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BottunTrigger : MonoBehaviour
{
    [Header("�M�~�b�N�ݒ�")]
    public GameObject KanzouGimmickParent;

    // �I�u�W�F�N�g�̕\�����ԁi�\���������鎞�ԁj
    public float displayDuration = 0.5f;

    [Header("�������")]
    private List<GameObject> gimmickChildren;
    private bool isRunning = false; // �M�~�b�N�����쒆���ǂ���

    [Header("�ڐG�ݒ�")]
    private bool isPlayerInRange = false;
    private const string PlayerTag = "Player"; // �v���C���[�̃^�O��


    // -----------------------------------------------------
    // ����������
    // -----------------------------------------------------

    void Start()
    {
        gimmickChildren = new List<GameObject>();

        if (KanzouGimmickParent != null)
        {
            // �S�Ă̎q�����X�g�ɒǉ����A�ŏ��͔�\���ɂ���
            foreach (Transform child in KanzouGimmickParent.transform)
            {
                gimmickChildren.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }

        // ������: ���̏�Ԃł́AplayerController�̎Q�ƁE�擾�����͊܂܂�Ă��܂���B��

        Debug.Log("BottunTrigger������������܂����B�q�I�u�W�F�N�g��: " + gimmickChildren.Count);
    }

    // -----------------------------------------------------
    // ���C���X�V����
    // -----------------------------------------------------

    void Update()
    {
        // 1. �͈͊O�A�܂��̓M�~�b�N���쒆�͏������I��
        if (!isPlayerInRange || isRunning)
        {
            return;
        }

        // 2. Enter�L�[�������ꂽ���`�F�b�N
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isRunning = true;

            // ���v���C���[��~�����͂����ɂ͊܂܂�܂���

            // �M�~�b�N����J�n
            StartCoroutine(DisplayObjectsSequentially());
        }
    }

    // -----------------------------------------------------
    // �M�~�b�N���� (�R���[�`��)
    // -----------------------------------------------------

    private IEnumerator DisplayObjectsSequentially()
    {
        WaitForSeconds waitDuration = new WaitForSeconds(displayDuration);

        // ���X�g���̊e�I�u�W�F�N�g�����Ԃɕ\���E��\��
        foreach (GameObject gimmick in gimmickChildren)
        {
            // 1. �\������
            Debug.Log($"�I�u�W�F�N�g��\��: {gimmick.name}");
            gimmick.SetActive(true);

            // �\�����Ԃ����ҋ@
            yield return waitDuration;

            // 2. ��\������
            gimmick.SetActive(false);
        }

        // �M�~�b�N����������
        isRunning = false;

        // ���v���C���[���A�����͂����ɂ͊܂܂�܂���

        Debug.Log("���ׂẴI�u�W�F�N�g�̕\���Ɛ؂�ւ����������܂����B");
    }

    // -----------------------------------------------------
    // �ڐG����
    // -----------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            isPlayerInRange = true;
            Debug.Log("�v���C���[���͈͓��ɓ���܂����B");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            isPlayerInRange = false;
            Debug.Log("�v���C���[���͈͊O�ɏo�܂����B");
        }
    }
}