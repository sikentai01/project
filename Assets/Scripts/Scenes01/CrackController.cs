using UnityEngine;
using System.Collections;

public class CrackController : MonoBehaviour
{
    public GameObject crackEffect;
    // playerMovementScript���C���X�y�N�^�[���犄�蓖�Ă����ɁA�X�N���v�g���Ŏ����Ō�������
    private MonoBehaviour playerMovementScript;

    void Start()
    {
        // �V�[���Ɋ֌W�Ȃ�GridMovement�X�N���v�g���������Ċ��蓖�Ă�
        playerMovementScript = FindObjectOfType<GridMovement>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (crackEffect != null)
            {
                crackEffect.SetActive(true);
            }

            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = false;
            }

            StartCoroutine(WaitAndResume());
            gameObject.SetActive(false);
        }
    }

    private IEnumerator WaitAndResume()
    {
        yield return new WaitForSeconds(1.0f);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }
}