using UnityEngine;

public class ConversationHub : MonoBehaviour
{
    public static ConversationHub Instance { get; private set; }

    [SerializeField] private ConversationRouter router;
    [SerializeField] private DialogueAdvanceInput advanceInput;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (!router) router = FindObjectOfType<ConversationRouter>(true);
        if (!advanceInput) advanceInput = FindObjectOfType<DialogueAdvanceInput>(true);
        // 必要なら次行を有効化：シーンを跨いでも常駐
        // DontDestroyOnLoad(gameObject);
    }

    public void Fire(string conversationId)
    {
        if (string.IsNullOrWhiteSpace(conversationId) || router == null) return;
        router.StartById(conversationId);
        if (advanceInput) advanceInput.SetActive(true); // Space受付ON
    }
}
