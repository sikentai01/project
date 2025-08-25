using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 1f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        int key = -1;

        // �A�j���[�V�����𐧌䂷�郍�W�b�N�𕪗�
        if (horizontalInput != 0 || verticalInput != 0)
        {
            animator.SetBool("Move_motion", true);
            if (horizontalInput > 0&&key!=2)
            {
                animator.SetInteger("Direction", 2);
                key = 2;
            }
            else if (horizontalInput < 0&&key!=1)
            {
                animator.SetInteger("Direction", 1);
                key = 1;
            }
            else if (verticalInput > 0 && key != 3)
            {
                animator.SetInteger("Direction", 3);
                key = 3;
            }
            else if (verticalInput < 0 && key != 0)
            {
                animator.SetInteger("Direction", 0);
                key = 0;
            }
        }
        else
        {
            animator.SetBool("Move_motion", false);
            key = -1;
        }

        // �ړ����W�b�N
        if (!isMoving && (horizontalInput != 0 || verticalInput != 0))
        {
            Vector3 nextPos = transform.position + new Vector3(horizontalInput * gridSize, verticalInput * gridSize, 0);

            if (!IsOccupied(nextPos))
            {
                targetPosition = nextPos;
                isMoving = true;
            }
        }

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * gridSize * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                isMoving = false;
            }
        }
    }

    bool IsOccupied(Vector3 nextPos)
    {
        Vector3 checkPos = nextPos + new Vector3(0, -0.5f, 0);
        Collider2D hitCollider = Physics2D.OverlapBox(checkPos, new Vector2(0.8f, 0.8f), 0f);

        if (hitCollider != null && hitCollider.gameObject != gameObject)
        {
            return true;
        }

        return false;
    }
}