using UnityEngine;

public class SequenceClickableObject : MonoBehaviour
{
    [Header("���̃I�u�W�F�N�g�̃C���f�b�N�X")]
    public int sequenceIndex;

    [Header("�N���b�N���ɍĐ�����SE")] // �� �V�K�ǉ��t�B�[���h
    public AudioClip clickSE;

    // �M�~�b�N�{�̂ւ̎Q��
    public ButtonSequenceGimmick targetGimmick;

    private void Start()
    {
        // ������Ԃ͔�\��
        gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (targetGimmick != null && targetGimmick.IsSequenceActive())
        {
            // ������ SE�Đ������̒ǉ� ������
            if (SoundManager.Instance != null && clickSE != null)
            {
                SoundManager.Instance.PlaySE(clickSE);
                Debug.Log($"[Clickable] Index {sequenceIndex} �̃N���b�NSE���Đ����܂����B");
            }

            // �M�~�b�N�{�̂ɃN���b�N��ʒm
            targetGimmick.OnButtonClick(sequenceIndex);
        }
    }
}