using UnityEngine;

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
            Debug.Log("Player entered trigger zone");
            isPlayerInside = true;
            playerMovement = other.GetComponent<GridMovement>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left trigger zone");
            isPlayerInside = false;
            playerMovement = null;
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            GridMovement move = other.GetComponent<GridMovement>();
            if (move != null)
            {
                Debug.Log("PlayerDir=" + move.GetDirection() + " / Need=" + requiredDirection);

                if (move.GetDirection() == requiredDirection)
                {
                    Debug.Log("ï˚å¸àÍív Å® CollectDocument()");
                    parent.CollectDocument();
                }
            }
        }
    }

    void Update()
    {
        if (isPlayerInside)
        {
            if (Input.anyKeyDown)
                Debug.Log("âΩÇ©ÉLÅ[âüÇ≥ÇÍÇΩ: " + Input.inputString);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E key pressed");

                if (playerMovement != null)
                {
                    Debug.Log("Direction = " + playerMovement.GetDirection() + ", Required = " + requiredDirection);

                    if (playerMovement.GetDirection() == requiredDirection)
                    {
                        Debug.Log("Direction matched Å® CollectDocument");
                        parent.CollectDocument();
                    }
                }
            }
        }
    }
}