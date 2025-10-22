using UnityEngine;

// GimmickBase���p�����A�����GameFlag�ƘA�����ĕ\���𐧌䂷��
public class FlagVisibilityGimmick : GimmickBase
{
    [Header("�\����؂�ւ���^�[�Q�b�g")]
    public GameObject targetObject;

    [Header("�\���ɕK�v��GameFlags��ID")]
    public string requiredFlagID;

    [Header("�t���O����������\������ (true) or ��\���ɂ��� (false)")]
    public bool activateOnFlag = true;

    private void Start()
    {
        if (string.IsNullOrEmpty(requiredFlagID))
        {
            Debug.LogError($"[FlagVisibilityGimmick] {gameObject.name}: Required Flag ID���ݒ肳��Ă��܂���B");
            return;
        }

        // �M�~�b�N�̏�����Ԃƃ��[�h��Ԃ̊m�F
        CheckFlagAndApplyVisibility();
    }

    private void Update()
    {
        // ���쒆�̃t���O�`�F�b�N�͕��ׂɂȂ�\�������邽�߁A
        // �p�ɂȐ؂�ւ���z�肵�Ȃ��ꍇ��Update�͕s�v�B
        // �������A�������f���K�v�ȏꍇ�͂��̃`�F�b�N���c���B
        CheckFlagAndApplyVisibility();
    }

    /// <summary>
    /// GameFlags�̏�Ԃ��m�F���A�\����Ԃ�K�p����
    /// </summary>
    // FlagVisibilityGimmick.cs
    public void CheckFlagAndApplyVisibility()
    {
        if (GameFlags.Instance == null) return;
        if (targetObject == null)
        {
            Debug.LogError($"[FlagVis] TargetObject��NULL�ł�: {gameObject.name}"); // �� �m�F�p���O1
            return;
        }

        bool hasFlag = GameFlags.Instance.HasFlag(requiredFlagID);
        bool shouldBeVisible = (hasFlag && activateOnFlag) || (!hasFlag && !activateOnFlag);

         // �� �m�F�p���O2

        // ���݂̕\����ԂƖڕW��Ԃ��قȂ�Ƃ��̂ݏ��������s
        if (targetObject.activeSelf != shouldBeVisible)
        {
            targetObject.SetActive(shouldBeVisible);

            this.currentStage = shouldBeVisible ? 1 : 0;

            // �� �������O
        }
    }

    // GimmickBase�̃��[�h�������I�[�o�[���C�h���A��Ԃ��ă`�F�b�N
    public override void LoadProgress(int stage)
    {
        // ���[�h����currentStage�͕�������邪�A�\����Ԃ̓t���O�Ɉˑ����邽�ߍă`�F�b�N
        base.LoadProgress(stage);
        CheckFlagAndApplyVisibility();
    }
}