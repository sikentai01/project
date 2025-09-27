using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConversationRouter : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        public string conversationId;
        public string relativePath; // 例: "story/intro_001.txt"
    }

    [Header("参照")]
    [SerializeField] private DialogueCore core;

    [Header("ID→StreamingAssets 相対パス")]
    [SerializeField] private List<Entry> registry = new List<Entry>();

    private readonly Queue<string> queue = new Queue<string>();
    private readonly HashSet<string> pendingIds = new HashSet<string>();
    private bool isActive = false;

    void Awake()
    {
        if (!core) core = GetComponentInChildren<DialogueCore>(true);
        if (!core) core = GetComponentInParent<DialogueCore>();
        if (!core) Debug.LogError("[ConversationRouter] DialogueCore が見つかりません。");

        if (core) core.OnConversationEnded += HandleConversationEnded;
    }

    public void StartById(string conversationId)
    {
        if (string.IsNullOrWhiteSpace(conversationId)) return;
        if (pendingIds.Contains(conversationId)) return; // 重複防止

        queue.Enqueue(conversationId);
        pendingIds.Add(conversationId);
        TryStartNext();
    }

    private void TryStartNext()
    {
        if (isActive || queue.Count == 0 || core == null) return;

        var id = queue.Dequeue();
        pendingIds.Remove(id);

        string rel = ResolveRelativePath(id);
        if (string.IsNullOrEmpty(rel))
        {
            Debug.LogWarning($"[Router] ID '{id}' に相対パス未登録。");
            TryStartNext();
            return;
        }

        string full = Path.Combine(Application.streamingAssetsPath, rel);

#if UNITY_ANDROID && !UNITY_EDITOR
        // Android は jar 内。必要なら UnityWebRequest で非同期読み込みに。
        StartCoroutine(LoadAndroid(full, id));
#else
        try
        {
            string text = File.ReadAllText(full, System.Text.Encoding.UTF8);
            isActive = true;
            core.StartConversation(id, text); // ← string 版を呼ぶ
        }
        catch (Exception e)
        {
            Debug.LogError($"[Router] 読み込み失敗: {full}\n{e}");
            TryStartNext();
        }
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private System.Collections.IEnumerator LoadAndroid(string fullPath, string id)
    {
        using (var req = UnityEngine.Networking.UnityWebRequest.Get(fullPath))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[Router] Android 読込失敗: {fullPath}\n{req.error}");
                TryStartNext();
                yield break;
            }
            isActive = true;
            core.StartConversation(id, req.downloadHandler.text);
        }
    }
#endif

    private string ResolveRelativePath(string id)
    {
        for (int i = 0; i < registry.Count; i++)
            if (registry[i].conversationId == id)
                return registry[i].relativePath;
        return null;
    }

    private void HandleConversationEnded(string id)
    {
        isActive = false;
        TryStartNext();
    }
}
