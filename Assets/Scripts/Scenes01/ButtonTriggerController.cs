using UnityEngine;

public class ButtonTriggerController : MonoBehaviour
{
    // �C���X�y�N�^�[����̑��M�~�b�N�̐e�I�u�W�F�N�g�����蓖�Ă�
    public Transform liverGimmickParent;

    // �M�~�b�N�̎q�I�u�W�F�N�g�̃��X�g�i����͎����Ŏ擾�j
    private GameObject[] gimmickChildren;

    // �v���C���[���g���K�[���ɂ��邩�ǂ���
    private bool isPlayerNear = false;

    void Start()
    {
        if (liverGimmickParent == null)
        {
            Debug.LogError("�̑��M�~�b�N�̐e�I�u�W�F�N�g�����蓖�Ă��Ă��܂���I");
            return;
        }

        // �̑��M�~�b�N�̎q�I�u�W�F�N�g��z��Ɋi�[
        int childCount = liverGimmickParent.childCount;
        gimmickChildren = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            gimmickChildren[i] = liverGimmickParent.GetChild(i).gameObject;
            // �S�Ă̎q�I�u�W�F�N�g���\���ɂ��Ă���
            gimmickChildren[i].SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // �ÓI�N���X���猻�݂̃C���f�b�N�X���擾
            int nextIndex = GimmickState.LiverGimmickIndex;

            if (nextIndex < gimmickChildren.Length)
            {
                // �Y���̎q�I�u�W�F�N�g��\��
                gimmickChildren[nextIndex].SetActive(true);

                // ���̃M�~�b�N�̂��߂ɃC���f�b�N�X���X�V
                GimmickState.LiverGimmickIndex++;

                Debug.Log($"�M�~�b�N {nextIndex + 1} ��\�����܂����B");
            }
            else
            {
                Debug.Log("�S�ẴM�~�b�N���������܂����B");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}