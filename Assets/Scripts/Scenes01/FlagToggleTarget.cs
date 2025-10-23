using UnityEngine;

public class FlagToggleTarget : MonoBehaviour
{
    [Header("�t���O�̏�Ԃ��Ď�����ID")]
    public string targetFlagID = "FLAG_WBC_ACTIVE";

    [Header("�؂�ւ��Ώۂ̃I�u�W�F�N�g")]
    public GameObject targetObject;

    [Header("�t���O���������� True �ɂ���iTrue:�\�� / False:��\���j")]
    public bool stateOnFlag = true;

    private bool previousFlagState = false;

    private void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError($"[FlagToggleTarget] {gameObject.name}: Target Object���ݒ肳��Ă��܂���B");
            this.enabled = false;
            return;
        }

        // ����`�F�b�N�Ə�����Ԃ̓K�p
        CheckFlagAndApplyState();
    }

    private void Update()
    {
        // ���t���[���`�F�b�N���āA�t���O��Ԃ̕ω��ɔ�������
        CheckFlagAndApplyState();
    }

    private void CheckFlagAndApplyState()
    {
        if (GameFlags.Instance == null) return;

        bool currentFlagState = GameFlags.Instance.HasFlag(targetFlagID);

        // ���݂̃^�[�Q�b�g�̃A�N�e�B�u���
        bool currentTargetActive = targetObject.activeSelf;

        // �t���O��Ԃ���Z�o�����ׂ��^�[�Q�b�g�̖ڕW���
        bool desiredState = currentFlagState == stateOnFlag; // �t���O����������ڕW��ԂɂȂ�

        // �t���O��Ԃ��ω��������A�܂��͌��݂̏�Ԃ��ڕW�ƈقȂ�Ƃ��ɂ̂ݏ���
        if (currentFlagState != previousFlagState || currentTargetActive != desiredState)
        {
            targetObject.SetActive(desiredState);
            previousFlagState = currentFlagState;

            Debug.Log($"[FlagToggleTarget] {targetFlagID} �ω����o�B{targetObject.name} �� {(desiredState ? "�\��" : "��\��")} �ɐ؂�ւ��܂����B");
        }
    }
}