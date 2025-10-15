using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BottunTrigger : MonoBehaviour
{
    [Header("�M�~�b�N�ݒ�")]
    public GameObject KanzouGimmickParent;

    // �I�u�W�F�N�g�̕\�����ԁi�\���������鎞�ԁj
    public float displayDuration = 0.5f;
    // ���̃I�u�W�F�N�g�֐؂�ւ��܂ł̑ҋ@���ԁi��\���ɂȂ��Ă��鎞�ԁj
    // ������͕\�����ԂƓ����l���g�p���܂��B

    [Header("�������")]
    private List<GameObject> gimmickChildren;
    private bool isRunning = false;


    void Start()
    {
        gimmickChildren = new List<GameObject>();

        if (KanzouGimmickParent != null)
        {
            // �S�Ă̎q�����X�g�ɒǉ����A��\���ɂ���
            foreach (Transform child in KanzouGimmickParent.transform)
            {
                gimmickChildren.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }

        Debug.Log("BottunTrigger������������܂����B�q�I�u�W�F�N�g��: " + gimmickChildren.Count);
    }

    void Update()
    {
        // Enter�L�[��������A�����ݏ��������s���łȂ����Ƃ��`�F�b�N
        if (Input.GetKeyDown(KeyCode.Return) && !isRunning)
        {
            isRunning = true;
            StartCoroutine(DisplayObjectsSequentially());
        }
    }

    // �R���[�`���F�I�u�W�F�N�g�����Ԃɕ\������\���ɂ��鏈��
    private IEnumerator DisplayObjectsSequentially()
    {
        // �ҋ@���Ԃ�ݒ�
        WaitForSeconds waitDuration = new WaitForSeconds(displayDuration);

        // ���X�g���̊e�I�u�W�F�N�g�����Ԃɏ���
        foreach (GameObject gimmick in gimmickChildren)
        {
            // �y1. �\�������z
            Debug.Log($"�I�u�W�F�N�g��\��: {gimmick.name}");
            gimmick.SetActive(true);

            // �\�����Ԃ����ҋ@
            yield return waitDuration;

            // �y2. ��\�������z
            gimmick.SetActive(false);

            // ���̃I�u�W�F�N�g��\������܂ł̑ҋ@���ԁi����͓������ԂŐݒ�j
            // �K�v�ɉ����āA�����ŕʂ̑ҋ@���ԁiyield return new WaitForSeconds(0.2f); �Ȃǁj��݂��Ă�������
        }

        // ���ׂĂ̏���������������t���O�����낷
        isRunning = false;
        Debug.Log("���ׂẴI�u�W�F�N�g�̕\���Ɛ؂�ւ����������܂����B");
    }
}