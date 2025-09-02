using UnityEngine;

public class AreaDarkeningController : MonoBehaviour
{
    public Material targetMaterial; // ���̃X�N���v�g�Ő��䂷��}�e���A��

    // �C���X�y�N�^�[�Őݒ肷��BoxCollider�̎Q��
    public BoxCollider areaCollider;

    void Start()
    {
        // BoxCollider�͈̔͂���Â�������W�͈͂�ݒ�
        if (targetMaterial != null && areaCollider != null)
        {
            targetMaterial.SetVector("_MinPoint", areaCollider.bounds.min);
            targetMaterial.SetVector("_MaxPoint", areaCollider.bounds.max);
        }
        else
        {
            Debug.LogError("�}�e���A���܂��̓R���C�_�[���ݒ肳��Ă��܂���B");
        }
    }
}
