using UnityEngine;
// �K�v�ɉ����� GimmickBase �N���X����`����Ă��閼�O��Ԃ�ǉ�
// using GimmickSystem; 

public class BreakWallTriggerGimmick : GimmickBase
{
    [Header("�\������I�u�W�F�N�g (�j�󂳂ꂽ��)")]
    public GameObject targetWallObject;

    // ���C��: ������Ԃ��Ǘ����邽�߂� private �t���O��ǉ���
    private bool isGimmickDone = false;

    // ... (NeedsItem, CanUseItem, UseItem �̕����͕ύX�Ȃ�) ...
    public override bool NeedsItem => false;

    public override bool CanUseItem(ItemData item)
    {
        return false;
    }

    public override void UseItem(ItemData usedItem, ItemTrigger trigger)
    {
        Debug.Log("���̃M�~�b�N�̓A�C�e���ł͍쓮���܂���B");
    }

    // -----------------------------------------------------
    // �M�~�b�N�̋N������
    // -----------------------------------------------------

    public override void StartGimmick(ItemTrigger trigger)
    {
        if (targetWallObject == null)
        {
            Debug.LogError("�^�[�Q�b�g�ƂȂ�ǃI�u�W�F�N�g���ݒ肳��Ă��܂���B");
            return;
        }

        // ���C��: IsCompleted �̑���ɁA���[�J���̃t���O�Ń`�F�b�N��
        if (isGimmickDone)
        {
            Debug.Log("�ǂ͊��ɔj�󂳂�Ă��܂��B");
            return;
        }

        // �ǃI�u�W�F�N�g��\���i�j�󂳂ꂽ��Ԃɂ���j
        targetWallObject.SetActive(true);
        Debug.Log($"[Gimmick] {targetWallObject.name} ��\�����A�ǂ�j�󂵂܂����B");

        // �M�~�b�N��������Ԃɂ���
        Complete(trigger);

        // ���ǉ�: �M�~�b�N�����t���O�𗧂Ă遚
        isGimmickDone = true;
    }
}