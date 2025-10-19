using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ConversationTrigger : MonoBehaviour
{
    [SerializeField] private ConversationTriggerAdapter adapter;
    [SerializeField] private string conversationId = "sample_001";
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private KeyCode key = KeyCode.E;
    [SerializeField] private bool autoStart = false;

    private bool inRange;
    private string lastColliderName;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        if (!adapter)
            adapter = ConversationTriggerAdapter.Instance 
                      ?? FindObjectOfType<ConversationTriggerAdapter>(true);
        Debug.Log($"[ConversationTrigger] Awake: scene={gameObject.scene.name}, adapter={(adapter ? adapter.name : "null")}");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;
        inRange = true;
        lastColliderName = other.name;

        if (autoStart)
            StartConv();
        else
            Debug.Log($"[ConversationTrigger] {other.name} が範囲内 (Scene:{gameObject.scene.name}) - {key}キーで開始可");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;
        inRange = false;
        Debug.Log($"[ConversationTrigger] {other.name} が範囲外 (Scene:{gameObject.scene.name})");
    }

    void Update()
    {
        if (!inRange || autoStart) return;
        if (Input.GetKeyDown(key))
        {
            Debug.Log($"[ConversationTrigger] {key} 押下 → 会話開始");
            StartConv();
        }
    }

    void StartConv()
    {
        if (!adapter)
            adapter = ConversationTriggerAdapter.Instance
                      ?? FindObjectOfType<ConversationTriggerAdapter>(true);

        if (!adapter)
        {
            Debug.LogWarning($"[ConversationTrigger] Adapter未設定 ({gameObject.scene.name}/{gameObject.name})");
            return;
        }

        if (string.IsNullOrWhiteSpace(conversationId))
        {
            Debug.Log($"[ConversationTrigger] FireDefault() 呼び出し → {adapter.name}");
            adapter.FireDefault();
        }
        else
        {
            Debug.Log($"[ConversationTrigger] Fire({conversationId}) 呼び出し → {adapter.name}");
            adapter.Fire(conversationId);
        }
    }
}
