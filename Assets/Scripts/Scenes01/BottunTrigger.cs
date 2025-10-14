using UnityEngine;
using System.Collections.Generic; // List<T>���g�����߂ɕK�v

public class BottunTrigger : MonoBehaviour
{
    [Header("�M�~�b�N�ݒ�")]
    // Inspector����A���Ԃɕ\���������q�I�u�W�F�N�g�̐e�ƂȂ�Q�[���I�u�W�F�N�g��ݒ肵�܂��B
    public GameObject KanzouGimmickParent;

    // Enter�L�[�̘A�ł�h�����߂̃N�[���_�E������
    public float inputDelay = 0.5f;

    [Header("�������")]
    // ���ݕ\������Ă���q�I�u�W�F�N�g�̃C���f�b�N�X�i0����n�܂�j
    private int currentIndex = 0;

    // �q�I�u�W�F�N�g���i�[���邽�߂̃��X�g
    private List<GameObject> gimmickChildren;

    // ���̓��͂��\�ɂȂ鎞��
    private float nextInputTime = 0f;


    void Start()
    {
        // 1. ���X�g�̏�����
        gimmickChildren = new List<GameObject>();

        if (KanzouGimmickParent != null)
        {
            // 2. �e�I�u�W�F�N�g�̎q��S�ă��X�g�ɒǉ�
            foreach (Transform child in KanzouGimmickParent.transform)
            {
                gimmickChildren.Add(child.gameObject);
                // 3. �ŏ��͑S�Ĕ�\���ɂ���
                child.gameObject.SetActive(false);
            }
        }

        Debug.Log("BottunTrigger������������܂����B�q�I�u�W�F�N�g��: " + gimmickChildren.Count);
    }

    void Update()
    {
        // 4. Enter�L�[�̓��͂ƃN�[���_�E�����`�F�b�N
        // �������ł́A�g���K�[�ɐڐG���Ă��邩�ǂ����̔���͏ȗ����Ă��܂��B
        //   �����g���K�[�ڐG���K�v�Ȃ�A�O��̐������Q�l�Ɏ������Ă��������B
        if (Input.GetKeyDown(KeyCode.Return) && Time.time > nextInputTime)
        {
            // 5. �q�I�u�W�F�N�g���܂��c���Ă��邩�i���̏ꍇ�A�C���f�b�N�X��3�ȉ����j���m�F
            // 4�̃I�u�W�F�N�g�̓C���f�b�N�X0, 1, 2, 3�ɑΉ����܂��B
            if (currentIndex < gimmickChildren.Count)
            {
                // 6. ���݂̃C���f�b�N�X�̎q�I�u�W�F�N�g��\��
                Debug.Log($"�I�u�W�F�N�g�̕\��: {gimmickChildren[currentIndex].name}");
                gimmickChildren[currentIndex].SetActive(true);

                // 7. ���̃I�u�W�F�N�g�փC���f�b�N�X��i�߂�
                currentIndex++;

                // 8. ���̓��͂��\�ɂȂ鎞�Ԃ�ݒ�
                nextInputTime = Time.time + inputDelay;
            }
            else
            {
                // 4�S�Ă�\�����I������ꍇ�̏���
                Debug.Log("���ׂẴI�u�W�F�N�g (4��) �̕\�����������܂����B");
                // �K�v�ɉ����āA�����Ŏ��̏����i��: �V�[���̐i�s�A�t���O�̃Z�b�g�Ȃǁj���L�q
            }
        }
    }
}