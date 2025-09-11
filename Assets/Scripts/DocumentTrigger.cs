using UnityEngine;

public class DocumentTrigger : MonoBehaviour
{
    [SerializeField] private DocumentObject parent; // InspectorÇ…ï\é¶Ç≥ÇÍÇÈ
    public int requiredDirection; // 0=â∫, 1=ç∂, 2=âE, 3=è„

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