using UnityEngine;

public class DialogueCanvasToggle : MonoBehaviour
{
    [SerializeField] private DialogueCore core;

    void Awake()
    {
        // Core自動取得（同Prefab内想定）
        if (!core) core = GetComponentInChildren<DialogueCore>(true);
        if (!core) core = GetComponentInParent<DialogueCore>();

        if (core)
        {
            // 会話が始まった“最初のページ確定”を OnSpeakerChanged で検知して表示ON
            core.OnSpeakerChanged += HandleShow;
            // 会話が終わったら表示OFF
            core.OnConversationEnded += HandleHide;
        }

        // シーン開始時は非表示
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (!core) return;
        core.OnSpeakerChanged    -= HandleShow;
        core.OnConversationEnded -= HandleHide;
    }

    private void HandleShow(string _)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    private void HandleHide(string _)
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }
}
