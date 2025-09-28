

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Sample : MonoBehaviour
{
    [SerializeField] ConversationTriggerAdapter adapter;
    [SerializeField] string conversationId = "sample_001";
    [SerializeField] string requiredTag = "Player";
    [SerializeField] KeyCode key = KeyCode.E;
    [SerializeField] bool autoStart = false;

    bool inRange;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        if (!adapter) adapter = FindObjectOfType<ConversationTriggerAdapter>(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;
        inRange = true;
        if (autoStart) StartConv();
        else Debug.Log("[Sample] 範囲内。Eで会話開始");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;
        inRange = false;
    }

    void Update()
    {
        if (!inRange || autoStart) return;
        if (Input.GetKeyDown(key)) StartConv();
    }

    void StartConv()
    {
        if (!adapter) { Debug.LogWarning("[Sample] Adapter未設定"); return; }
        if (!string.IsNullOrWhiteSpace(conversationId)) adapter.Fire(conversationId);
        else adapter.FireDefault();
    }
}
