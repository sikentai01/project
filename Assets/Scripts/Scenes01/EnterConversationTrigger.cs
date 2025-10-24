using UnityEngine;

// 必須要件としてCollider2Dを自動的にアタッチします
[RequireComponent(typeof(Collider2D))]
public class EnterConversationTrigger : MonoBehaviour
{
    // ConversationTriggerAdapter は ConversationHub との橋渡し役です
    [SerializeField] private ConversationTriggerAdapter adapter;

    // 開始する会話のID（ConversationRouter.csで定義）
    [SerializeField] private string conversationId = "sample_001";

    // 接触を判定するオブジェクトのタグ（通常は "Player"）
    [SerializeField] private string requiredTag = "Player";

    // ★ ここを KeyCode.Return (Enterキー) に変更します
    [SerializeField] private KeyCode key = KeyCode.Return;

    // 接触時にキー入力なしで即座に開始するかどうか
    [SerializeField] private bool autoStart = false;

    private bool inRange;
    private string lastColliderName;

    void Reset()
    {
        // Collider2Dをトリガーに設定
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        // Adapterを自動検索
        if (!adapter)
            adapter = ConversationTriggerAdapter.Instance
                      ?? FindObjectOfType<ConversationTriggerAdapter>(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;
        inRange = true;
        lastColliderName = other.name;

        if (autoStart)
            StartConv();
        else
            Debug.Log($"[EnterConversationTrigger] {other.name} が範囲内 - {key}キーで開始可");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;
        inRange = false;
        Debug.Log($"[EnterConversationTrigger] {other.name} が範囲外");
    }

    void Update()
    {
        // 範囲内にいて、かつ自動開始でない場合にキー入力をチェック
        if (!inRange || autoStart) return;
        if (Input.GetKeyDown(key))
        {
            Debug.Log($"[EnterConversationTrigger] {key} 押下 → 会話開始");
            StartConv();
        }
    }

    void StartConv()
    {
        if (!adapter)
        {
            Debug.LogWarning($"[EnterConversationTrigger] Adapter未設定");
            return;
        }

        // 外部依存コード: プレイヤー移動とメニュー禁止
        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player ? player.GetComponent<GridMovement>() : null;
        if (move != null) move.enabled = false;

        // PauseMenu.blockMenu は定義されていませんが、既存コードに合わせます
        // PauseMenu.blockMenu = true; 

        // ConversationHub.cs を経由して会話開始を通知
        if (string.IsNullOrWhiteSpace(conversationId))
        {
            // IDがない場合は、Adapterのデフォルト処理を呼び出す想定
            // Debug.Log($"[EnterConversationTrigger] FireDefault() 呼び出し");
            // adapter.FireDefault(); 
        }
        else
        {
            // IDを指定して開始
            Debug.Log($"[EnterConversationTrigger] Fire({conversationId}) 呼び出し");
            adapter.Fire(conversationId);
        }

        // 会話終了イベントを購読
        if (DialogueCore.Instance != null)
        {
            DialogueCore.Instance.OnConversationEnded += OnConversationEnded;
        }
    }

    private void OnConversationEnded(string id)
    {
        // 終了したら移動・メニューを戻す
        // PauseMenu.blockMenu = false;

        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player ? player.GetComponent<GridMovement>() : null;
        if (move != null) move.enabled = true;

        // 登録解除
        if (DialogueCore.Instance != null)
            DialogueCore.Instance.OnConversationEnded -= OnConversationEnded;

        Debug.Log($"[EnterConversationTrigger] 会話終了 → 操作再開 ({id})");
    }
}