using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 1f;

    [Header("�����ݒ�")]
    public AudioClip tileFootstep;
    public AudioClip matFootstep;
    public AudioClip dirtFootstep;
    public AudioClip woodFootstep; // �� �؂̑����ǉ�
    public AudioClip GlassFootstep;
    public float stepInterval = 0.35f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Animator animator;

    private int currentDirection = 0;
    private float footstepTimer = 0f; // �����Ԋu�p�^�C�}�[

    void Start()
    {
        animator = GetComponent<Animator>();
        targetPosition = transform.position;
    }

    void Update()
    {
        if (PauseMenu.isPaused || GameOverController.isGameOver || TitleManager.isTitleActive)
        {
            animator.SetBool("Move_motion", false);
            return;
        }
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // �A�j���[�V�����𐧌䂷�郍�W�b�N
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
            //�����Ă��Ȃ���
            animator.SetBool("Move_motion", false);
        }

        // �ړ����W�b�N

        // �c�Ɖ��̓��͂������ɂ���ꍇ������悤�ɂ���
        if (horizontalInput != 0 && verticalInput != 0)
        {
            horizontalInput = 0;
            verticalInput = 0;
            animator.SetBool("Move_motion", false);
        }

        //�ȉ��ړ�����
        if (!isMoving && (horizontalInput != 0 || verticalInput != 0))
        {
            Vector3 nextPos = transform.position + new Vector3(horizontalInput * gridSize, verticalInput * gridSize, 0);

            //�ړ���
            if (!IsOccupied(nextPos))
            {
                targetPosition = nextPos;
                isMoving = true;
            }
            //�ړ��s�A��Q���Ƃ��ˁA���[�V�����Ƃ߂邾��
            else
            {
                animator.SetBool("Move_motion", false);
            }
        }

        //��������ړ�
        // �_�b�V���I�I
        float currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            // Shift�L�[��������Ă���ꍇ�A���x1.7�{�I�����I
            currentMoveSpeed = moveSpeed * 1.7f;
        }
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentMoveSpeed * gridSize * Time.deltaTime);

            // ��������i�ړ����̂݁j
            footstepTimer += Time.deltaTime;
            if (footstepTimer >= stepInterval)
            {
                PlayFootstepSound();
                footstepTimer = 0f;
            }

            if (transform.position == targetPosition)
            {
                isMoving = false;
            }
        }
    }

    // ---- ������炷���� ----
    void PlayFootstepSound()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.0f);
        if (hit.collider == null) return;

        FloorType floor = hit.collider.GetComponent<FloorType>();
        if (floor == null || SoundManager.Instance == null) return;

        AudioClip clip = null;
        switch (floor.surfaceType)
        {
            case FloorType.SurfaceType.Mat: clip = matFootstep; break;
            case FloorType.SurfaceType.Dirt: clip = dirtFootstep; break;
            case FloorType.SurfaceType.Wood: clip = woodFootstep; break;
            case FloorType.SurfaceType.Glass: clip = GlassFootstep; break;
            default: clip = tileFootstep; break;
        }

        if (clip != null)
            SoundManager.Instance.PlaySE(clip);
    }

    //�����蔻�蓙
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

    //�ȉ��A�C�e���̊Ǌ�
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

    public void SetPositionAndDirection(Vector3 position, int direction)
    {
        transform.position = position;
        SetDirection(direction);
        UpdateDirectionVisual();
    }

    private void UpdateDirectionVisual()
    {
        // �����ڂ̌���������X�v���C�g���]�Ȃǁi���g�p�ł�OK�j
    }
}