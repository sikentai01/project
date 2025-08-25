using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 1f;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool Move_motion = true;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (!isMoving)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 nextPos = Vector3.zero;

            if (horizontalInput != 0)
            {
                nextPos = transform.position + new Vector3(horizontalInput * gridSize, 0, 0);
                animator.SetBool("Move_motion", true);
                if (horizontalInput > 0) animator.SetInteger("Direction", 2); // �E
                else if (horizontalInput < 0) animator.SetInteger("Direction", 1); // ��
            }
            else if (verticalInput != 0)
            {
                nextPos = transform.position + new Vector3(0, verticalInput * gridSize, 0);
                animator.SetBool("Move_motion", true);
                if (verticalInput > 0) animator.SetInteger("Direction", 3); // ��
                else if (verticalInput < 0) animator.SetInteger("Direction", 0); // ��
            }

            // ���̏ꏊ�Ɉړ��\���`�F�b�N
            if (nextPos != Vector3.zero && !IsOccupied(nextPos))
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
        // nextPos�̒��S���班��Y�������ɂ��炷
        Vector3 checkPos = nextPos + new Vector3(0, -0.5f, 0); // �L�����N�^�[�̑������`�F�b�N

        // Physics2D.OverlapBox���g�p���A�R���C�_�[�̃T�C�Y�𒲐�
        Collider2D hitCollider = Physics2D.OverlapBox(checkPos, new Vector2(0.8f, 0.8f), 0f);

        if (hitCollider != null && hitCollider.gameObject != gameObject)
        {
            return true;
        }

        return false;
    }
}
