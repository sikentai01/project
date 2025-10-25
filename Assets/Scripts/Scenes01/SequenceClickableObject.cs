using UnityEngine;

public class SequenceClickableObject : MonoBehaviour
{
    [Header("���̃I�u�W�F�N�g�̃C���f�b�N�X")]
    public int sequenceIndex;

    [Header("�N���b�N���ɍĐ�����SE (�C��)")]
    public AudioClip clickSE;

    // �M�~�b�N�{�̂ւ̎Q��
    public ButtonSequenceGimmick targetGimmick;

    // ������ �C���ӏ�: Awake�Ŕ�\������������ ������
    private void Awake()
    {
        // Awake��Start����Ɏ��s����邽�߁AInspector�̐ݒ���㏑�����A�����ɔ�\����ۏ؂���
        gameObject.SetActive(false);
    }
    // ������ �����܂ŏC�� ������

    private void OnMouseDown()
    {
        if (targetGimmick != null && targetGimmick.IsSequenceActive())
        {
            // SE�Đ�����
            if (SoundManager.Instance != null && clickSE != null)
            {
                SoundManager.Instance.PlaySE(clickSE);
            }

            // �M�~�b�N�{�̂ɃN���b�N��ʒm
            targetGimmick.OnButtonClick(sequenceIndex);
            Debug.Log($"[Clickable] Index {sequenceIndex} ���N���b�N���܂����B");
        }
    }

    // Start() ���\�b�h�͕s�v�ɂȂ������ߍ폜���܂�
}