using UnityEngine;

// GimmickBase���p�����A�\���E��\���𐧌䂷��
public class VisibilityGimmick : GimmickBase
{
    [Header("�\����؂�ւ���^�[�Q�b�g")]
    public GameObject targetObject;

    /// <summary>
    /// �M�~�b�N�̏�Ԃ��X�V����
    /// stage: 0=��\��, 1=�\��
    /// </summary>
    /// <param name="stage">�V�����X�e�[�W</param>
    public void SetVisibility(int stage)
    {
        if (targetObject == null)
        {
            Debug.LogWarning($"[VisibilityGimmick] TargetObject���ݒ肳��Ă��܂���BID: {gimmickID}");
            return;
        }

        // stage 1�Ȃ�\���A0�Ȃ��\��
        bool isVisible = (stage == 1);
        targetObject.SetActive(isVisible);
        this.currentStage = stage; // GimmickBase�̐i�s�i�K���X�V

        Debug.Log($"[VisibilityGimmick] {targetObject.name} �̕\���� {(isVisible ? "ON" : "OFF")} �ɂ��܂����B (Stage: {stage})");
    }

    // ���[�h���ɏ�Ԃ𕜌�
    public override void LoadProgress(int stage)
    {
        base.LoadProgress(stage);
        // ���[�h���ɂ��\����Ԃ𔽉f������
        SetVisibility(stage);
    }

    private void Awake()
    {
        // �Q�[���J�n���Ɍ��݂�stage�Ɋ�Â��ď����\����ݒ�iLoadProgress�ŏ㏑�������\������j
        if (targetObject != null)
        {
            SetVisibility(this.currentStage);
        }
    }
}