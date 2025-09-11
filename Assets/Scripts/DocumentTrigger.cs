using UnityEngine;

public class DocumentTrigger : MonoBehaviour
{
    [SerializeField] private DocumentObject parent; // Inspector�ɕ\�������
    public int requiredDirection; // 0=��, 1=��, 2=�E, 3=��

    public void Setup(DocumentObject parentObject, int dir)
    {
        parent = parentObject;
        requiredDirection = dir;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            GridMovement move = other.GetComponent<GridMovement>();
            if (move != null && move.GetDirection() == requiredDirection)
            {
                parent.CollectDocument();
            }
        }
    }
}