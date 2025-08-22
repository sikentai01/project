using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Grid")]
    public float tilePixels = 32f;       // �^�C���̃s�N�Z���T�C�Y
    public float pixelsPerUnit = 32f;   // Sprite Importer �� PPU
    public float moveSpeed = 3f;         // �ړ����x�i���������j
    private float tileSize;              // ���[���h���W�ł�1�^�C���� = tilePixels / pixelsPerUnit

    [Header("Animation")]
    public int walkFrames = 4;           // ���s�A�j���̃t���[����
    public int fps = 12;                 // �A�j����FPS

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

        // �����ʒu���O���b�h�ɃX�i�b�v
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

        // �ǃ`�F�b�N
        if (obstacleMask.value != 0)
        {
            if (Physics2D.OverlapBox(target, checkSize, 0f, obstacleMask) != null)
            {
                animator.SetBool("Moving", false);
                return false;
            }
        }

        // �A�j���[�V��������
        animator.SetBool("Moving", true);

        if (dir.x > 0) animator.SetInteger("Direction", 2);   // �E
        else if (dir.x < 0) animator.SetInteger("Direction", 1); // ��
        else if (dir.y > 0) animator.SetInteger("Direction", 3); // ��
        else if (dir.y < 0) animator.SetInteger("Direction", 0); // ��

        StartCoroutine(MoveRoutine(start, target));
        return true;
    }

    IEnumerator MoveRoutine(Vector2 start, Vector2 target)
    {
        isMoving = true;

        // 1�� = 1���[�V�����T�C�N��
        float duration = (float)walkFrames / fps;  // ��: 4�� �� 12fps = 0.33�b

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
