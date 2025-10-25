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
    public float stepInterval = 0.1f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Animator animator;

    private int currentDirection = 0;
    private float footstepTimer = 0f; // �����Ԋu�p�^�C�}�[
    private AudioClip currentFootstepClip; // �� ���������L������ϐ���ǉ�

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
                DetermineFootstepSound(targetPosition);
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
        if (currentFootstepClip != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(currentFootstepClip);
        }
    }
    //--�ړ���̑����𔻒肷��
    void DetermineFootstepSound(Vector3 destination)
    {
        int layerMask = LayerMask.GetMask("Ground");

        // IsOccupied �Ɠ��l�ɁA�X�v���C�g���S(destination)���� -1.0f �I�t�Z�b�g�����u�����v����ɂ���
        Vector3 rayStartPos = destination + new Vector3(0, -0.5f, 0);

        // Raycast�́u�����v����^���ɒZ�����ĂΏ\���i��F0.5f�j
        RaycastHit2D hit = Physics2D.Raycast(rayStartPos, Vector2.down, 0.5f, layerMask);

        // --- ���� PlayFootstepSound �̒��g�������ɈڐA ---
        if (hit.collider == null)
        {
            currentFootstepClip = tileFootstep; // ������Ȃ���΃f�t�H���g
            return;
        }

        FloorType floor = hit.collider.GetComponent<FloorType>();
        if (floor == null)
        {
            currentFootstepClip = tileFootstep; // FloorType���Ȃ���΃f�t�H���g
            return;
        }

        // clip �ł͂Ȃ� currentFootstepClip �ɋL��������
        switch (floor.surfaceType)
        {
            case FloorType.SurfaceType.Mat: currentFootstepClip = matFootstep; break;
            case FloorType.SurfaceType.Dirt: currentFootstepClip = dirtFootstep; break;
            case FloorType.SurfaceType.Wood: currentFootstepClip = woodFootstep; break;
            case FloorType.SurfaceType.Glass: currentFootstepClip = GlassFootstep; break;
            default: currentFootstepClip = tileFootstep; break;
        }
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
    public void ForceStopMovement()
    {
        isMoving = false;
        targetPosition = transform.position;
        footstepTimer = 0f;
        // �A�j���[�V�������m���ɒ�~
        if (animator != null)
        {
            animator.SetBool("Move_motion", false);
        }
    }

}