using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ConversationTrigger : MonoBehaviour
{
    [SerializeField] private ConversationTriggerAdapter adapter;
    [SerializeField] private string conversationId = "sample_001";
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private KeyCode key = KeyCode.E;
    [SerializeField] private bool autoStart = false;

    [Header("向き指定 ( -1 = 制限なし / 0=下,1=左,2=右,3=上 )")]
    [SerializeField] private int requiredDirection = -1; // ★ インスペクターで設定可

    private bool inRange = false;
    private bool isConversationActive = false;
    private bool hasStarted = false;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    private void Awake()
    {
        if (!adapter)
            adapter = ConversationTriggerAdapter.Instance ?? FindObjectOfType<ConversationTriggerAdapter>(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;
        inRange = true;

        if (autoStart && !isConversationActive)
            StartConv();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;
        inRange = false;
    }

    private void Update()
    {
        // 会話中 or 自動開始中は無視
        if (PauseMenu.isPaused||!inRange || autoStart || isConversationActive) return;

        // Eキー or Enterキーどちらでも開始できる
        if ((Input.GetKeyDown(key) || Input.GetKeyDown(KeyCode.Return)) && !hasStarted)
        {
            hasStarted = true;

            // ★ 向きチェック
            if (!IsFacingRequiredDirection())
            {
                Debug.Log($"[ConversationTrigger] 向きが違うため会話不可 (必要:{requiredDirection})");
                Invoke(nameof(ResetHasStarted), 0.1f);
                return;
            }

            StartConv();
            Invoke(nameof(ResetHasStarted), 0.1f); // 押しっぱなし対策
        }
    }

    private bool IsFacingRequiredDirection()
    {
        if (requiredDirection == -1) return true; // 向き制限なし

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        var move = player.GetComponent<GridMovement>();
        if (move == null) return false;

        return move.GetDirection() == requiredDirection;
    }

    private void ResetHasStarted()
    {
        hasStarted = false;
    }

    private void StartConv()
    {
        if (isConversationActive) return;
        isConversationActive = true;

        if (!adapter)
            adapter = ConversationTriggerAdapter.Instance ?? FindObjectOfType<ConversationTriggerAdapter>(true);

        if (!adapter)
        {
            Debug.LogWarning($"[ConversationTrigger] Adapter未設定 ({gameObject.scene.name}/{gameObject.name})");
            isConversationActive = false;
            return;
        }

        // プレイヤー操作ロック
        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player ? player.GetComponent<GridMovement>() : null;
        if (move != null) move.enabled = false;
        PauseMenu.blockMenu = true;

        // 会話イベント開始
        if (string.IsNullOrWhiteSpace(conversationId))
            adapter.FireDefault();
        else
            adapter.Fire(conversationId);

        // 終了イベント登録
        if (DialogueCore.Instance != null)
        {
            DialogueCore.Instance.OnConversationEnded -= OnConversationEnded;
            DialogueCore.Instance.OnConversationEnded += OnConversationEnded;
        }
    }

    private void OnConversationEnded(string id)
    {
        // 会話終了で操作を戻す
        PauseMenu.blockMenu = false;
        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player ? player.GetComponent<GridMovement>() : null;
        if (move != null) move.enabled = true;

        // 登録解除
        if (DialogueCore.Instance != null)
            DialogueCore.Instance.OnConversationEnded -= OnConversationEnded;

        isConversationActive = false;
        Debug.Log($"[ConversationTrigger] 会話終了 → 再入力可能 ({id})");
    }
}
