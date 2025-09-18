using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 1f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Animator animator;

    private int currentDirection = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (PauseMenu.isPaused)
        {
            // ポーズ中はアニメーションを停止
            animator.SetBool("Move_motion", false);
            return; // 以降の処理をすべてスキップ
        }
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // アニメーションを制御するロジックを分離
        if (horizontalInput != 0 || verticalInput != 0)
        {
            animator.SetBool("Move_motion", true);
            if (verticalInput == 0)
            {
                if (horizontalInput > 0)
                {
                    currentDirection = 2;
                    animator.SetInteger("Direction", 2);
                }
                else if (horizontalInput < 0)
                {
                    currentDirection = 1;
                    animator.SetInteger("Direction", 1);
                }
            }
            if (horizontalInput == 0)
            {
                if (verticalInput > 0)
                {
                    currentDirection = 3;
                    animator.SetInteger("Direction", 3);
                }
                else if (verticalInput < 0)
                {
                    currentDirection = 0;
                    animator.SetInteger("Direction", 0);
                }
            }
        }
        else
        {
            animator.SetBool("Move_motion", false);
        }

        // 移動ロジック
        if (!isMoving && (horizontalInput != 0 || verticalInput != 0))
        {
            Vector3 nextPos = transform.position + new Vector3(horizontalInput * gridSize, verticalInput * gridSize, 0);

            if (!IsOccupied(nextPos))
            {
                targetPosition = nextPos;
                isMoving = true;
            }
            else
            {
                animator.SetBool("Move_motion", false);
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

        if (hitCollider != null && !hitCollider.isTrigger && hitCollider.gameObject != gameObject)
        {
            return true;
        }

        return false;
    }

    public int GetDirection()
    {
        return currentDirection;
    }

    public void SetDirection(int dir)
    {
        currentDirection = dir;
        if (animator != null)
        {
            animator.SetInteger("Direction", dir);
        }
    }
}