using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Grid")]
    public float tilePixels = 32f;       // タイルのピクセルサイズ
    public float pixelsPerUnit = 32f;   // Sprite Importer の PPU
    public float moveSpeed = 3f;         // 移動速度（無視される）
    private float tileSize;              // ワールド座標での1タイル長 = tilePixels / pixelsPerUnit

    [Header("Animation")]
    public int walkFrames = 4;           // 歩行アニメのフレーム数
    public int fps = 12;                 // アニメのFPS

    [Header("Collision (optional)")]
    public LayerMask obstacleMask;
    public Vector2 checkSize = new Vector2(0.8f, 0.8f);

    private Rigidbody2D rb;
    private Animator animator;

    private bool isMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        tileSize = tilePixels / pixelsPerUnit;

        // 初期位置をグリッドにスナップ
        rb.position = transform.position;
    }

    void Update()
    {
        Vector2Int dir = ReadHeldDirection();

        if (dir != Vector2Int.zero && !isMoving)
        {
            TryStartMove(dir);
        }
        else if (dir == Vector2Int.zero && !isMoving)
        {
            animator.SetBool("Moving", false);
        }
    }

    Vector2Int ReadHeldDirection()
    {
        int x = 0, y = 0;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) x = 1;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) x = -1;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) y = 1;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) y = -1;

        return new Vector2Int(x, y);
    }

    bool TryStartMove(Vector2Int dir)
    {
        if (isMoving) return false;

        Vector2 start = rb.position;
        Vector2 target = start + (Vector2)dir * tileSize;

        // 壁チェック
        if (obstacleMask.value != 0)
        {
            if (Physics2D.OverlapBox(target, checkSize, 0f, obstacleMask) != null)
            {
                animator.SetBool("Moving", false);
                return false;
            }
        }

        // アニメーション制御
        animator.SetBool("Moving", true);

        if (dir.x > 0) animator.SetInteger("Direction", 2);   // 右
        else if (dir.x < 0) animator.SetInteger("Direction", 1); // 左
        else if (dir.y > 0) animator.SetInteger("Direction", 3); // 上
        else if (dir.y < 0) animator.SetInteger("Direction", 0); // 下

        StartCoroutine(MoveRoutine(start, target));
        return true;
    }

    IEnumerator MoveRoutine(Vector2 start, Vector2 target)
    {
        isMoving = true;

        // 1歩 = 1モーションサイクル
        float duration = (float)walkFrames / fps;  // 例: 4枚 ÷ 12fps = 0.33秒

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            rb.MovePosition(Vector2.Lerp(start, target, Mathf.Clamp01(t)));
            yield return null;
        }

        rb.MovePosition(SnapToGrid(target));

        isMoving = false;
        animator.SetBool("Moving", false);
    }

    Vector2 SnapToGrid(Vector2 pos)
    {
        float x = Mathf.Round(pos.x / tileSize) * tileSize;
        float y = Mathf.Round(pos.y / tileSize) * tileSize;
        return new Vector2(x, y);
    }

    void OnDrawGizmosSelected()
    {
        if (obstacleMask.value == 0) return;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.DrawWireCube(
            Application.isPlaying ? (Vector3)(rb ? rb.position : (Vector2)transform.position) : transform.position,
            (Vector3)checkSize
        );
    }
}
