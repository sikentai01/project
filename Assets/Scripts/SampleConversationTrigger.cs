using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SampleConversationTrigger : MonoBehaviour
{
    [Header("会話開始先")]
    [SerializeField] private ConversationRouter router;
    [SerializeField] private DialogueAdvanceInput advanceInput;

    [Header("会話ID（Router.registry と一致）")]
    [SerializeField] private string conversationId = "sample_001";

    private bool inRange = false;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        if (!router) router = FindObjectOfType<ConversationRouter>(true);
        if (!advanceInput) advanceInput = FindObjectOfType<DialogueAdvanceInput>(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        inRange = true;
        Debug.Log("[SampleTrigger2D] 範囲内に入りました。Eキーで会話開始。");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        inRange = false;
    }

    void Update()
    {
        if (!inRange) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartConversation();
        }
    }

    private void StartConversation()
    {
        if (!router || string.IsNullOrWhiteSpace(conversationId))
        {
            Debug.LogWarning("[SampleTrigger2D] Router未設定またはID未設定です。");
            return;
        }
        router.StartById(conversationId);
        if (advanceInput) advanceInput.SetActive(true);
        Debug.Log($"[SampleTrigger2D] 会話開始: {conversationId}");
    }
}
