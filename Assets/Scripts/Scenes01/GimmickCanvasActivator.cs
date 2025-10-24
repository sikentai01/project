using UnityEngine;

public class GimmickCanvasActivator : MonoBehaviour
{
    [Header("�\������GimmickCanvas�̃��[�g�I�u�W�F�N�g")]
    [Tooltip("�V�[������GimmickCanvas�v���n�u�̃��[�g�����蓖�Ă�")]
    public GameObject gimmickCanvasRoot;

    private GimmickTrigger targetTrigger;

    private void Start()
    {
        // ���g�Ɠ����Q�[���I�u�W�F�N�g�ɃA�^�b�`����Ă���GimmickTrigger���擾
        targetTrigger = GetComponent<GimmickTrigger>();
        
        if (gimmickCanvasRoot == null)
        {
            Debug.LogError("[Activator] GimmickCanvasRoot���ݒ肳��Ă��܂���B");
            this.enabled = false;
        }

        // ������Ԃ��\���ɐݒ�
        if (gimmickCanvasRoot != null && gimmickCanvasRoot.activeSelf)
        {
            gimmickCanvasRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (targetTrigger == null || gimmickCanvasRoot == null) return;

        // GimmickTrigger��IsPlayerNear�v���p�e�B�i�����ŊǗ�����Ă���j��true�ł����Canvas��\��
        // IsPlayerNear��private set�̂��߁AGimmickTrigger��public���\�b�h�o�R�ł�����Ԃ��擾�ł��܂���B
        // �� GimmickTrigger.cs��IsPlayerNear��public�łȂ��ꍇ�A���̒��ړI�ȎQ�Ƃ͂ł��܂���B

        // ������ �b��I�ȑ�֎�i�FGimmickTrigger.cs��public�ȏ�Ԏ擾���\�b�h���Ăяo�� ������
        // �����ł́AGimmickTrigger.cs�� IsPlayerNear�v���p�e�B�����݂��A
        // ���� ItemTrigger.cs �̂悤�� Update �œ��͑҂������Ă���M�~�b�N�ł���Ɖ��肵�A
        // �v���C���[���͈͊O�ɏo�Ă����Canvas���\���ɂ���悤�ɐ��䂵�܂��B
        
        // ���ۂɂ́AGimmickTrigger���p������N���X�iItemTrigger�Ȃǁj���A
        // �v���C���[���߂Â����Ƃ��ɉ��炩�̃t���O�𗧂Ă郍�W�b�N�������Ă���͂��ł��B

        // ����́AGimmickTrigger.targetGimmick��currentStage��0�̏ꍇ�ɂ̂݃��j���[��\�����郍�W�b�N��g�݂܂��B
        
        GimmickBase targetGimmick = targetTrigger.targetGimmick;
        
        if (targetGimmick == null) return;

        // �M�~�b�N�� Stage 0�i������ԁj�ŁA�v���C���[���߂��ɂ���ꍇ�̂݃��j���[��\������
        // IsPlayerNear��public�łȂ����߁AInput�ŊԐړI�Ɍ��m�����܂ŕ\�����܂���B

        // �yGimmickTrigger���v���C���[�ɐڋ߂������Ƃ����m����ł��m���ȕ��@�z
        // GimmickTrigger.cs�𒼐ڂ����炸�A���� IsPlayerNear �� public set �łȂ����߁A
        // GimmickTrigger�ɃA�^�b�`����Ă���R���C�_�[�����g�Ŏ擾���A�ă`�F�b�N���郍�W�b�N�ɐ؂�ւ��܂��B
        
        Collider2D triggerCollider = GetComponent<Collider2D>();

        if (triggerCollider != null)
        {
            // Physics2D.IsTouching(Player��Collider, ���g��Collider) �Ń`�F�b�N���܂����A
            // ����͌������������߁AStartConv()�Ȃǂ̃^�C�~���O�Ő��䂷������ǂ��ł��B
            
            // �� �V���v���ɁACanvas�̕\���E��\���͊O���̃C�x���g�ɔC����݌v�ɕύX���܂� ��
        }
    }
}