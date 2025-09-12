using UnityEngine;
using UnityEngine.EventSystems;

public class DocumentTrigger : MonoBehaviour
{
    public DocumentObject parent;
    public int requiredDirection;

    private bool isPlayerInside = false;
    private GridMovement playerMovement;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerMovement = other.GetComponent<GridMovement>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerMovement = null;
        }
    }

    void Update()
    {
        // �v���C���[���͈͓��ɂ��鎞�̂�
        if (isPlayerInside)
        {
            // �|�[�Y�� or UI���쒆�Ȃ珈�����Ȃ�
            if (PauseMenu.isPaused) return;
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null) return;

            // Enter�L�[�ŏE������
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (playerMovement != null && playerMovement.GetDirection() == requiredDirection)
                {
                    parent.CollectDocument();
                }
            }
        }
    }
}