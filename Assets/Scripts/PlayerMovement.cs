using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 1f;

    [Header("足音設定")]
    public AudioClip tileFootstep;
    public AudioClip matFootstep;
    public AudioClip dirtFootstep;
    public AudioClip woodFootstep; // ← 木の足音追加
    public AudioClip GlassFootstep;
    public float stepInterval = 0.1f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Animator animator;

    private int currentDirection = 0;
    private float footstepTimer = 0f; // 足音間隔用タイマー
    private AudioClip currentFootstepClip; // ← ★足音を記憶する変数を追加

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

        // アニメーションを制御するロジック
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
            //動いていない時
            animator.SetBool("Move_motion", false);
        }

        // 移動ロジック

        // 縦と横の入力が同時にある場合動かんようにした
        if (horizontalInput != 0 && verticalInput != 0)
        {
            horizontalInput = 0;
            verticalInput = 0;
            animator.SetBool("Move_motion", false);
        }

        //以下移動判定
        if (!isMoving && (horizontalInput != 0 || verticalInput != 0))
        {
            Vector3 nextPos = transform.position + new Vector3(horizontalInput * gridSize, verticalInput * gridSize, 0);

            //移動可
            if (!IsOccupied(nextPos))
            {
                targetPosition = nextPos;
                isMoving = true;
                DetermineFootstepSound(targetPosition);
            }
            //移動不可、障害物とかね、モーションとめるだけ
            else
            {
                animator.SetBool("Move_motion", false);
            }
        }

        //ここから移動
        // ダッシュ！！
        float currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            // Shiftキーが押されている場合、速度1.7倍！お得！
            currentMoveSpeed = moveSpeed * 1.7f;
        }
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentMoveSpeed * gridSize * Time.deltaTime);

            // 足音制御（移動中のみ）
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

    // ---- 足音を鳴らす処理 ----
    void PlayFootstepSound()
    {
        if (currentFootstepClip != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(currentFootstepClip);
        }
    }
    //--移動先の足音を判定する
    void DetermineFootstepSound(Vector3 destination)
    {
        int layerMask = LayerMask.GetMask("Ground");

        // IsOccupied と同様に、スプライト中心(destination)から -1.0f オフセットした「足元」を基準にする
        Vector3 rayStartPos = destination + new Vector3(0, -0.5f, 0);

        // Raycastは「足元」から真下に短く撃てば十分（例：0.5f）
        RaycastHit2D hit = Physics2D.Raycast(rayStartPos, Vector2.down, 0.5f, layerMask);

        // --- 元の PlayFootstepSound の中身をここに移植 ---
        if (hit.collider == null)
        {
            currentFootstepClip = tileFootstep; // 見つからなければデフォルト
            return;
        }

        FloorType floor = hit.collider.GetComponent<FloorType>();
        if (floor == null)
        {
            currentFootstepClip = tileFootstep; // FloorTypeがなければデフォルト
            return;
        }

        // clip ではなく currentFootstepClip に記憶させる
        switch (floor.surfaceType)
        {
            case FloorType.SurfaceType.Mat: currentFootstepClip = matFootstep; break;
            case FloorType.SurfaceType.Dirt: currentFootstepClip = dirtFootstep; break;
            case FloorType.SurfaceType.Wood: currentFootstepClip = woodFootstep; break;
            case FloorType.SurfaceType.Glass: currentFootstepClip = GlassFootstep; break;
            default: currentFootstepClip = tileFootstep; break;
        }
    }

    //当たり判定等
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

    //以下アイテムの管轄
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
        // 見た目の向き調整やスプライト反転など（未使用でもOK）
    }
    public void ForceStopMovement()
    {
        isMoving = false;
        targetPosition = transform.position;
        footstepTimer = 0f;
        // アニメーションも確実に停止
        if (animator != null)
        {
            animator.SetBool("Move_motion", false);
        }
    }

}