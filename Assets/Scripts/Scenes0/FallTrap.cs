using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FallTrap : MonoBehaviour
{
    private Collider2D trapCollider;

    void OnEnable()
    {
        trapCollider = GetComponent<Collider2D>();

        // �͂��߂��玞�ɂ͋���ON�iSaveTriggered�������Ȃ�g���K�[�L���j
        if (GameFlags.Instance == null || !GameFlags.Instance.HasFlag("SaveTriggered"))
        {
            trapCollider.isTrigger = true;
            Debug.Log("[FallTrap] �t���O�Ȃ� �� �g���K�[�ėL����");
        }
        else
        {
            trapCollider.isTrigger = false;
            Debug.Log("[FallTrap] �Z�[�u�� �� �g���K�[�������i�ǁj");
        }
    }

    void Start()
    {
        trapCollider = GetComponent<Collider2D>();

        // ���łɃt���O�������Ă���i��: �Z�[�u��j�Ȃ�㩂𖳌���
        if (GameFlags.Instance != null && GameFlags.Instance.HasFlag("SaveTriggered"))
        {
            DisableTrap();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �g���K�[�łȂ��ꍇ�͖���
        if (!trapCollider.isTrigger) return;

        //  BootLoader�Ńv���C���[�����ړ����Ȃ甭�����Ȃ�
        if (BootLoader.IsPlayerSpawning)
        {
            Debug.Log("[FallTrap] �v���C���[�����ړ��� �� ����");
            return;
        }

        // ���ł�GameOver�J�ڒ��Ȃ�d���h�~
        if (BootLoader.IsTransitioning)
        {
            Debug.Log("[FallTrap] �V�[���J�ڒ��̂��ߖ���");
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("[FallTrap] �v���C���[�����Ƃ����ɗ��� �� GameOver��");

            // �v���C���[�̑���𖳌���
            var move = other.GetComponent<GridMovement>();
            if (move != null)
            {
                move.ForceStopMovement(); // �� �m���ɒ�~
                move.enabled = false;
                Debug.Log("[FallTrap] �v���C���[������~���܂���");
            }

            // �����x�点�ăQ�[���I�[�o�[��ʂ�\��
            StartCoroutine(ShowGameOverRoutine());
        }
    }

    private IEnumerator ShowGameOverRoutine()
    {
        // �������o�Ȃǂ̂��ߏ����ҋ@
        yield return new WaitForSeconds(0.5f);

        Debug.Log("[FallTrap] GameOver�V�[���֐ؑ֗v��");
        BootLoader.Instance.SwitchSceneInstant("GameOver"); // �� BootLoader�̑��ؑփ��\�b�h���g�p
    }

    // ====== ���Ƃ������u�������v ======
    public void DisableTrap()
    {
        if (trapCollider != null)
        {
            trapCollider.isTrigger = false; // �ǈ����ɂ���
            Debug.Log("[FallTrap] ���Ƃ����𖳌��� �� �ǂɂ��܂���");
        }
    }
}
