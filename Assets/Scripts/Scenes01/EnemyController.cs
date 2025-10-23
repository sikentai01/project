using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("�ړ����x")]
    public float moveSpeed = 1.5f;

    [Header("�ǐՊJ�n�܂ł̒x�����ԁi�b�j")]
    public float startDelayTime = 1.5f; // �P�\����

    private Transform targetPlayer;
    private bool isTracking = false;

    // EnemyController.cs �� Start() ���\�b�h�i�ŏI�C���Łj

    private void Start()
    {
        // 1. �v���C���[��Transform������
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            targetPlayer = playerObj.transform;
        }
        else
        {
            Debug.LogError("[EnemyController] 'Player'�^�O�����I�u�W�F�N�g��������܂���B");
            this.enabled = false;
            return;
        }

        // 2. �������d�v������ Start() ���玩���ŒǐՂ��J�n����R���[�`���Ăяo�����폜�I
        // StartCoroutine(StartTrackingAfterDelay()); // ���̍s�͍폜
    }

    /// <summary>
    /// �O������ǐՂ��J�n������iGlassStepGimmick����Ă΂��j
    /// </summary>
    public void StartTracking()
    {
        if (!isTracking)
        {
            // �P�\���Ԃ�݂��邽�߂̃R���[�`�����N��
            StartCoroutine(StartTrackingAfterDelay());
        }
    }

    private IEnumerator StartTrackingAfterDelay()
    {
        isTracking = false;
        // �P�\���Ԃ�ݒ�
        yield return new WaitForSeconds(startDelayTime);
        isTracking = true;
        Debug.Log("[EnemyController] �P�\���ԏI���B�ǐՂ��J�n���܂��B");
    }

    private void Update()
    {
        if (targetPlayer == null) return;

        // �ǐՃt���O�� true �̏ꍇ�݈̂ړ������s
        if (!isTracking) return;

        // �v���C���[�Ɍ������Ĉړ�
        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}