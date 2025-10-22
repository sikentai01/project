using UnityEngine;

// Unity�̊K�w�\�����Q�l�ɁAGimmickBase�ƘA�g����Controller
public class CrackController : MonoBehaviour
{
    [Header("�Ώۂ̃M�~�b�NID (VisibilityGimmick�ɐݒ肵�����̂ƈ�v������)")]
    public string targetGimmickID;

    [Header("�\��/��\����؂�ւ���M�~�b�N�{��")]
    public VisibilityGimmick targetGimmick;

    private void Start()
    {
        // === �M�~�b�N�{�̂̌������� ===
        if (targetGimmick == null)
        {
            var gimmicks = Object.FindObjectsByType<VisibilityGimmick>(FindObjectsSortMode.None);
            foreach (var g in gimmicks)
            {
                if (g.gimmickID == targetGimmickID)
                {
                    targetGimmick = g;
                    break;
                }
            }
        }

        if (targetGimmick == null)
        {
            Debug.LogError($"[CrackController] ID:{targetGimmickID} �ɑΉ����� VisibilityGimmick ��������܂���I");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetGimmick != null)
        {
            // ���ɕ\���ς݁icurrentStage��1�ȏ�j�Ȃ牽�����Ȃ�
            if (targetGimmick.currentStage >= 1)
                return;

            // �������̏�ԂȂ̂ŁA�\�������s (stage=1)
            targetGimmick.SetVisibility(1);

            Debug.Log($"[CrackController] �v���C���[���N�� �� Inside-Town-E_12��\��");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 1��\�������ł́A��x�\���������\���ɖ߂��Ȃ����߁A���ɏ����͍s���܂���B
    }
}