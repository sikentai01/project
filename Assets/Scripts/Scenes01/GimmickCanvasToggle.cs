using UnityEngine;

public class GimmickCanvasToggle : MonoBehaviour
{
    [SerializeField] private DialogueCore core;

    void Awake()
    {
        // Coreの自動取得（DialogueCanvasToggle.csと同じロジック）
        if (!core) core = GetComponentInChildren<DialogueCore>(true);
        if (!core) core = GetComponentInParent<DialogueCore>();

        if (core)
        {
            // 会話が始まったら表示ON
            core.OnSpeakerChanged += HandleShow;

            // 会話が終わったら表示OFF
            core.OnConversationEnded += HandleHide;
        }
        else
        {
            Debug.LogWarning("[GimmickCanvasToggle] DialogueCore が見つかりませんでした。");
        }

        // ★修正点★ シーン開始時は非表示にする
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (!core) return;
        core.OnSpeakerChanged -= HandleShow;
        core.OnConversationEnded -= HandleHide;
    }

    private void HandleShow(string _)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        Debug.Log("[GimmickCanvasToggle] 会話開始 → GimmickCanvas を表示");
    }

    private void HandleHide(string _)
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
        Debug.Log("[GimmickCanvasToggle] 会話終了 → GimmickCanvas を非表示");
    }
}