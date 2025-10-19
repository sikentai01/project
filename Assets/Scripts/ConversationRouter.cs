using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConversationRouter : MonoBehaviour
{
    [Serializable]
    public class Entry
    {
        [Header("会話ID (ConversationTriggerのIDと一致させる)")]
        public string conversationId;

        [Header("StreamingAssets 相対パス (例: story/intro_001.txt)")]
        public string relativePath;

        [Header("直接参照する場合はこちらにドラッグ (任意)")]
        public TextAsset textAsset;
    }

    [Header("参照")]
    [SerializeField] private DialogueCore core;

    [Header("会話ID → ファイル参照")]
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

        // ① TextAsset参照 or ② Registry相対パス or ③ ID自動探索 の順に解決
        string text = ResolveConversationText(id);

        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning($"[Router] ID '{id}' に対応する会話データが見つかりません。");
            TryStartNext();
            return;
        }

        isActive = true;
        core.StartConversation(id, text);
    }

    private string ResolveConversationText(string id)
    {
        // ① 登録されたEntryを検索
        foreach (var e in registry)
        {
            if (e.conversationId == id)
            {
                // TextAsset優先
                if (e.textAsset != null)
                {
                    Debug.Log($"[Router] TextAsset '{e.textAsset.name}' から読み込み (ID:{id})");
                    return e.textAsset.text;
                }

                // 相対パス指定がある場合
                if (!string.IsNullOrEmpty(e.relativePath))
                {
                    string full = Path.Combine(Application.streamingAssetsPath, e.relativePath);
                    if (File.Exists(full))
                    {
                        Debug.Log($"[Router] StreamingAssets/{e.relativePath} から読み込み (ID:{id})");
                        return File.ReadAllText(full, System.Text.Encoding.UTF8);
                    }
                }

                break;
            }
        }

        // ② 登録なし：IDをそのままファイル名として扱う
        string auto = Path.Combine(Application.streamingAssetsPath, id + ".txt");
        if (File.Exists(auto))
        {
            Debug.Log($"[Router] 登録なし: '{id}.txt' を自動検出");
            return File.ReadAllText(auto, System.Text.Encoding.UTF8);
        }

        return null;
    }

    private void HandleConversationEnded(string id)
    {
        isActive = false;
        TryStartNext();
    }
}
