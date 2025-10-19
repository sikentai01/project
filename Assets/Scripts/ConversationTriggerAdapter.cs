using UnityEngine;
using UnityEngine.SceneManagement;

public class ConversationTriggerAdapter : MonoBehaviour
{
    public static ConversationTriggerAdapter Instance { get; private set; }

    [SerializeField] private ConversationRouter router;
    [SerializeField] private DialogueAdvanceInput advanceInput;
    [SerializeField] private string defaultConversationId;

    void Awake()
    {
        // ★シングルトン＋常駐
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!router)       router       = FindObjectOfType<ConversationRouter>(true);
        if (!advanceInput) advanceInput = FindObjectOfType<DialogueAdvanceInput>(true);

        Debug.Log($"[Adapter] scene={gameObject.scene.name}, router={(router?router.name:"null")}, adv={(advanceInput?advanceInput.name:"null")}");
        // もし各部屋でRouterが切り替わる設計なら、sceneLoadedで再バインドする：
        SceneManager.sceneLoaded += (_, __) =>
        {
            if (!router)       router       = FindObjectOfType<ConversationRouter>(true);
            if (!advanceInput) advanceInput = FindObjectOfType<DialogueAdvanceInput>(true);
        };
    }

    public void FireDefault()
    {
        if (!string.IsNullOrWhiteSpace(defaultConversationId))
            Fire(defaultConversationId);
    }

    // 引数付きで直接呼べる
    public void Fire(string conversationId)
    {
        if (string.IsNullOrWhiteSpace(conversationId) || router == null) {
            Debug.LogWarning($"[Adapter] Fire blocked: id='{conversationId}', router={(router?"OK":"null")}");
            return;
        }
        Debug.Log($"[Adapter] StartById('{conversationId}')");
        router.StartById(conversationId);
        if (advanceInput) advanceInput.SetActive(true);
    }
}
