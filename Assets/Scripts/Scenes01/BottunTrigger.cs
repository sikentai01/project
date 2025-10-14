using UnityEngine;
using System.Collections.Generic;

public class BottunTrigger : MonoBehaviour
{
    // �̑��M�~�b�N�̐e�I�u�W�F�N�g�iInspector����h���b�O&�h���b�v�Őݒ�j
    public GameObject KanzouGimmickParent;

    // ���ݕ\������Ă���q�I�u�W�F�N�g�̃C���f�b�N�X
    private int currentIndex = 0;

    // �q�I�u�W�F�N�g�̃��X�g�i�L���b�V���p�j
    private List<GameObject> gimmickChildren;

    // ���͌�̃N�[���_�E�����ԁi�A�Ŗh�~�p�j
    private float nextInputTime = 0f;
    public float inputDelay = 0.5f; // ��: 0.5�b�̒x��
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // �q�I�u�W�F�N�g�̃��X�g��������
        gimmickChildren = new List<GameObject>();

        if (KanzouGimmickParent != null)
        {
            // �e�I�u�W�F�N�g�̎q��S�Ď擾
            foreach (Transform child in KanzouGimmickParent.transform)
            {
                gimmickChildren.Add(child.gameObject);
                // �ŏ��͑S�Ĕ�\���ɂ���
                child.gameObject.SetActive(false);
            }
        }
        currentIndex = 0; // �C���f�b�N�X�����Z�b�g
    }

    // Update is called once per frame
    void Update()
    {
        // ��BottunTrigger���v���C���[�ɐڐG���Ă��邱�Ƃ������t���O�i��: isPlayerInRange�j��
        //   true�̎��̂݁A���̏��������s����悤�ɂ���̂���ʓI�ł��B

        // Enter�L�[�������ꂽ���A�܂��͎w�肳�ꂽ���́i��: Input.GetKeyDown("return")�j���`�F�b�N
        // ����сA�N�[���_�E�����Ԃ��o�߂��������`�F�b�N
        if (Input.GetKeyDown(KeyCode.Return) && Time.time > nextInputTime)
        {
            // �q�I�u�W�F�N�g���܂��c���Ă��邩���m�F
            if (currentIndex < gimmickChildren.Count)
            {
                // ���݂̃C���f�b�N�X�̎q�I�u�W�F�N�g��\��
                gimmickChildren[currentIndex].SetActive(true);

                // ���̃I�u�W�F�N�g�փC���f�b�N�X��i�߂�
                currentIndex++;

                // ���̓��͂��\�ɂȂ鎞�Ԃ�ݒ�
                nextInputTime = Time.time + inputDelay;
            }
            else
            {
                // ���ׂĕ\�����I������ꍇ�̏����i��: ���炩�̃��b�Z�[�W�\���⎟�̃t�F�[�Y�ֈڍs�j
                Debug.Log("�̑��M�~�b�N�̕\�����������܂����B");
            }
        }
    }
}
