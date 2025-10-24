using UnityEngine;

public class ConversationTriggerAdapter : MonoBehaviour
{
    public static ConversationTriggerAdapter Instance { get; private set; }

    [Header("接続")]
    [SerializeField] private ConversationRouter router;
    [SerializeField] private DialogueAdvanceInput advanceInput;

    [Header("デフォルト会話ID")]
    [SerializeField] private string defaultConversationId;

    void Awake()
    {
        if (Instance != null && Instance != this) { Instance = this; } else { Instance = this; }
        if (!router) router = FindObjectOfType<ConversationRouter>(true);
        if (!advanceInput) advanceInput = FindObjectOfType<DialogueAdvanceInput>(true);
    }

    // ID指定（通常ルート）
    public void Fire(string conversationId)
    {
        if (string.IsNullOrWhiteSpace(conversationId)) return;
        if (router == null) { Debug.LogWarning("[Adapter] Router 未設定"); return; }
        router.StartById(conversationId);
        if (advanceInput) advanceInput.SetActive(true);
    }

    // デフォルトID
    public void FireDefault()
    {
        if (!string.IsNullOrWhiteSpace(defaultConversationId)) Fire(defaultConversationId);
        else Debug.LogWarning("[Adapter] defaultConversationId 未設定");
    }

    // TextAsset 直流し（任意）
    public void FireTextAsset(TextAsset asset, string id, bool systemWindow = false)
    {
        if (!asset) { Debug.LogWarning("[Adapter] TextAsset null"); return; }
        FireRawText(asset.text, id, systemWindow);
    }

    // ★生テキスト直流し（今回 ItemTrigger が呼ぶやつ）
    public void FireRawText(string text, string id = "system", bool systemWindow = false)
    {
        if (string.IsNullOrWhiteSpace(text)) { Debug.LogWarning("[Adapter] text 空"); return; }
        var core = FindObjectOfType<DialogueCore>(true);
        if (!core) { Debug.LogWarning("[Adapter] DialogueCore 見つからない"); return; }
        core.StartConversation(id, text);               // まずは「出すだけ」
        if (advanceInput) advanceInput.SetActive(true);
    }
}
