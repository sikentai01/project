using UnityEngine;

public class GlassStepGimmick : GimmickBase
{
    [Header("�M�~�b�NID (�Z�[�u�p)")]
    public string gimmickID;

    [Header("���[�v/�t���O�����ɕK�v�ȉ�")]
    public int requiredSteps = 5;

    [Header("���[�v������^�[�Q�b�g�I�u�W�F�N�g (WhiteBloodCell)")]
    public GameObject targetObjectToActivate; // ���[�v�Ώ�

    [Header("�����ɕK�v��GameFlags��ID (��: PoisonTrigger)")]
    public string requiredFlagID = "PoisonTrigger";

    [Header("�N�����ɗ��Ă�t���OID")]
    public string activeFlagID = "FLAG_WBC_ACTIVE"; // ���[�v�J�n�������t���O

    // �v���C���[�ւ̎Q�Ƃ̓��[�v�ɕK�v�Ȃ̂ŁA�ÓI�Ɏ擾���܂�
    private Transform playerTransform;

    // ���[�v���\�ȏ�ԁi�t���O�������A�񐔂��������ꂽ�j���ǂ���
    private bool isWarpReady = false;

    private void Start()
    {
        // �v���C���[�I�u�W�F�N�g��Transform��ÓI�Ɏ擾
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        // �M�~�b�N���������Ă��Ȃ��ꍇ�͏�����ԁi��\���j��ۏ�
        if (targetObjectToActivate != null)
        {
            targetObjectToActivate.SetActive(false);
        }

        // ���[�h���܂��͏�����ԂŊ��ɏ����𖞂����Ă��邩�`�F�b�N
        if (currentStage >= requiredSteps)
        {
            // �����𖞂����Ă��邪�A���[�v���͓̂��ނ��тɎ��s�����ׂ��Ȃ̂ŁA�����ł͎��s���Ȃ��B
            // ��Ԃ����� 'Ready' �ɂ���B
            isWarpReady = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnGlassStepped();
        }
    }

    /// <summary>
    /// �K���X�𓥂񂾂Ƃ��ɌĂяo�����\�b�h
    /// </summary>
    public void OnGlassStepped()
    {
        // 1. �O��t���O�������Ă��邩�`�F�b�N
        bool hasRequiredFlag = GameFlags.Instance != null && GameFlags.Instance.HasFlag(requiredFlagID);

        // 2. �K�v�ȃt���O�������Ă��Ȃ��ꍇ�́A�������I��
        if (!hasRequiredFlag)
        {
            Debug.Log($"[GlassStepGimmick] �O��t���O '{requiredFlagID}' �������Ă��Ȃ����߁A���[�v�������X�L�b�v���܂��B");
            return;
        }

        // 3. �i�s�x�i���j�񐔁j�𑝂₷
        this.currentStage++;
        Debug.Log($"[GlassStepGimmick] �K���X�𓥂񂾉�: {currentStage} / {requiredSteps}");

        // 4. ���[�v�̏����������`�F�b�N
        if (this.currentStage >= requiredSteps)
        {
            isWarpReady = true;
            TryWarpToPlayer();
        }
    }

    private void TryWarpToPlayer()
    {
        // ���[�v���������A�^�[�Q�b�g�A�v���C���[���S�đ����Ă��邩�`�F�b�N
        if (!isWarpReady || targetObjectToActivate == null || playerTransform == null)
        {
            return;
        }

        // A. WhiteBloodCell���A�N�e�B�u�ɂ���
        if (!targetObjectToActivate.activeSelf)
        {
            targetObjectToActivate.SetActive(true);
            Debug.Log($"[GlassStepGimmick] {targetObjectToActivate.name} ���N�����܂����B");

            // B. ���߂ċN�������Ƃ��Ƀt���O�𗧂Ă�
            if (!string.IsNullOrEmpty(activeFlagID))
            {
                GameFlags.Instance?.SetFlag(activeFlagID);
                Debug.Log($"[GlassStepGimmick] �B���t���O '{activeFlagID}' �𗧂Ă܂����B");
            }
        }

        // C. WhiteBloodCell���v���C���[�̈ʒu�Ƀ��[�v������
        targetObjectToActivate.transform.position = playerTransform.position;
        Debug.Log($"[GlassStepGimmick] {targetObjectToActivate.name} ���v���C���[�̈ʒu�Ƀ��[�v�����܂����B");

        // ���[�v��̈ړ����X���[�Y�ɂȂ�悤�A�������Z�𓯊�������i�C�Ӂj
        Physics2D.SyncTransforms();
    }

    // =====================================================
    // �Z�[�u�E���[�h�̏���
    // =====================================================
    public override void LoadProgress(int stage)
    {
        currentStage = stage;

        if (currentStage >= requiredSteps)
        {
            // ���[�h���̓��[�v����������Ԃɂ���
            isWarpReady = true;

            // WhiteBloodCell�̏�Ԃ��Č��i�A�N�e�B�u�ɂ���j
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(true);
            }

            // �B���t���O�𕜌�
            if (!string.IsNullOrEmpty(activeFlagID))
            {
                GameFlags.Instance?.SetFlag(activeFlagID);
            }
        }
        else
        {
            // ���������͏�����Ԃɖ߂�
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(false);
            }
        }
    }
}