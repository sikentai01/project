using UnityEngine;

public class ConversationTriggerAdapter : MonoBehaviour
{
    [SerializeField] private ConversationRouter router;
    [SerializeField] private DialogueAdvanceInput advanceInput;
    [SerializeField] private string defaultConversationId;

    void Awake()
    {
        if (!router) router = FindObjectOfType<ConversationRouter>(true);
        if (!advanceInput) advanceInput = FindObjectOfType<DialogueAdvanceInput>(true);
    }

    // Button / Timeline / AnimationEvent から使う用（InspectorでIDを入れておく）
    public void FireDefault()
    {
        if (!string.IsNullOrWhiteSpace(defaultConversationId))
            Fire(defaultConversationId);
    }

    // 引数付きで直接呼べる
    public void Fire(string conversationId)
    {
        if (string.IsNullOrWhiteSpace(conversationId) || router == null) return;
        router.StartById(conversationId);
        if (advanceInput) advanceInput.SetActive(true);
    }
}
