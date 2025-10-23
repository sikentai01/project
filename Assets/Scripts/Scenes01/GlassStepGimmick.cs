using UnityEngine;

public class GlassStepGimmick : GimmickBase
{
    [Header("�M�~�b�NID (�Z�[�u�p)")]
    public string gimmickID;

    [Header("�ǐՎ҂�L�������邽�߂ɕK�v�ȉ�")]
    public int requiredSteps = 5;

    [Header("�\���ɂ���^�[�Q�b�g�I�u�W�F�N�g (WhiteBloodCell)")]
    public GameObject targetObjectToActivate;

    [Header("�����ɕK�v��GameFlags��ID (��: PoisonTrigger)")]
    public string requiredFlagID = "PoisonTrigger";

    [Header("�N�����ɗ��Ă�t���OID")]
    public string activeFlagID = "FLAG_WBC_ACTIVE"; // WhiteBloodCell�N���������t���O

    private bool isGimmickCompleted = false;

    private void Start()
    {
        // ... (�ȗ�: Start() �̏���������) ...
        if (currentStage >= requiredSteps)
        {
            TryCompleteGimmick();
        }
        else
        {
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnGlassStepped();
        }
    }

    public void OnGlassStepped()
    {
        if (isGimmickCompleted) return;

        bool hasRequiredFlag = GameFlags.Instance != null && GameFlags.Instance.HasFlag(requiredFlagID);

        if (!hasRequiredFlag)
        {
            Debug.Log($"[GlassStepGimmick] �O��t���O '{requiredFlagID}' �������Ă��Ȃ����߁A�J�E���g���X�L�b�v���܂��B");
            return;
        }

        this.currentStage++;
        Debug.Log($"[GlassStepGimmick] �K���X�𓥂񂾉�: {currentStage} / {requiredSteps}");

        TryCompleteGimmick();
    }

    private void TryCompleteGimmick()
    {
        bool enoughSteps = this.currentStage >= requiredSteps;

        if (enoughSteps)
        {
            // A. WhiteBloodCell (�ǐՎ�) ��\���ɂ���
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(true);
                Debug.Log($"[GlassStepGimmick] {targetObjectToActivate.name} ��\���ɂ��܂����B");

                // �� �ǐՊJ�n�̃t���O�𗧂Ă�
                if (!string.IsNullOrEmpty(activeFlagID))
                {
                    GameFlags.Instance?.SetFlag(activeFlagID);
                    Debug.Log($"[GlassStepGimmick] �B���t���O '{activeFlagID}' �𗧂Ă܂����B");
                }

                // �� �ǐՎ҂̈ړ���ON�ɂ���I
                var enemyController = targetObjectToActivate.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.StartTracking();
                }
            }

            // --- �������� ---
            isGimmickCompleted = true;
            this.enabled = false;
            Debug.Log("[GlassStepGimmick] ���������B���I�M�~�b�N���������܂����B");
        }
    }

    public override void LoadProgress(int stage)
    {
        currentStage = stage;

        if (currentStage >= requiredSteps)
        {
            // WhiteBloodCell�̏�Ԃ��Č�
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(true);

                // ���[�h�����ǐՂ��ĊJ������
                var enemyController = targetObjectToActivate.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.StartTracking();
                }
            }

            // �B���t���O�𕜌�
            if (!string.IsNullOrEmpty(activeFlagID))
            {
                GameFlags.Instance?.SetFlag(activeFlagID);
            }

            isGimmickCompleted = true;
            this.enabled = false;
        }
        else
        {
            if (targetObjectToActivate != null)
            {
                targetObjectToActivate.SetActive(false);
            }
        }
    }
}