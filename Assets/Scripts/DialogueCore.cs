using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueCore : MonoBehaviour
{
    public static DialogueCore Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ===== イベント群 =====
    public event Action<string> OnSpeakerChanged;
    public event Action<string[]> OnLinesReady;
    public event Action<string> OnConversationEnded;
    public event Action<string> OnPortraitChanged;

    private string currentConversationId;
    private readonly List<Page> pages = new List<Page>();
    private int pageIndex = -1;

    [Serializable]
    private class Page
    {
        public string speaker;
        public List<string> lines = new List<string>(); // 最大3行
    }

    // ===== 会話開始 =====
    public void StartConversation(string id, TextAsset textAsset)
    {
        if (textAsset == null) { Debug.LogWarning("[DialogueCore] TextAsset=null"); Finish(id); return; }
        StartConversation(id, textAsset.text);
    }

    public void StartConversation(string id, string rawText)
    {
        currentConversationId = id;
        pages.Clear();
        pageIndex = -1;

        BuildPages(rawText);
        if (pages.Count == 0) { Finish(id); return; }
        ShowPage(0);
    }

    public void NextPage()
    {
        if (pages.Count == 0) return;
        int next = pageIndex + 1;
        if (next >= pages.Count) { Finish(currentConversationId); return; }
        ShowPage(next);
    }

    // ===== 解析処理 =====
    private void BuildPages(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return;

        string text = raw.Replace("\r\n", "\n").Replace("\r", "\n");
        var lines = text.Split('\n');
        string currentSpeaker = null;
        Page currentPage = null;

        void FlushPageIfAny()
        {
            if (currentPage != null && currentPage.lines.Count > 0)
                pages.Add(currentPage);
            currentPage = null;
        }

        for (int i = 0; i < lines.Length; i++)
        {
            string line = NormalizeLine(lines[i]);

            // 空行 = ブロック区切り
            if (string.IsNullOrEmpty(line))
            {
                FlushPageIfAny();
                continue;
            }

            // コマンド（表示しない）
            if (IsCommand(line))
            {
                ExecuteCommand(line);
                continue;
            }

            // 「名前: 本文」なら話者切替を判定
            if (TryParseSpeakerLine(line, out var maybeSpeaker, out var maybeBody))
            {
                if (!string.IsNullOrEmpty(currentSpeaker) && currentSpeaker != maybeSpeaker)
                    FlushPageIfAny(); // 混在不可 → 改ページ

                currentSpeaker = maybeSpeaker;

                if (!string.IsNullOrEmpty(maybeBody))
                {
                    EnsurePage(ref currentPage, currentSpeaker);
                    AddLineAndSplitIfNeeded(ref currentPage, maybeBody);
                }
                continue;
            }

            // 通常行：現在の話者で追加（未設定ならナレーション）
            if (string.IsNullOrEmpty(currentSpeaker))
                currentSpeaker = "Narration";

            EnsurePage(ref currentPage, currentSpeaker);
            AddLineAndSplitIfNeeded(ref currentPage, line);
        }

        FlushPageIfAny();
    }

    private static string NormalizeLine(string s)
    {
        if (s == null) return string.Empty;
        s = s.Replace('\u3000', ' ');
        return s.Trim();
    }

    private static bool TryParseSpeakerLine(string line, out string speaker, out string body)
    {
        int idx = line.IndexOf(':');
        if (idx > 0)
        {
            speaker = line.Substring(0, idx).Trim();
            body = line.Substring(idx + 1).Trim();
            return !string.IsNullOrEmpty(speaker);
        }
        speaker = null;
        body = null;
        return false;
    }

    private static bool IsCommand(string line) => line.StartsWith("#");

    // ===== コマンド処理 =====
    private void ExecuteCommand(string line)
    {
        if (!line.StartsWith("#")) return;

        string cmdLine = line.Trim();
        string[] parts = cmdLine.Substring(1).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return;

        string command = parts[0].ToLowerInvariant();
        string args = parts.Length > 1 ? parts[1] : "";

        switch (command)
        {
            case "give":
                CmdGiveItem(args);
                break;

            default:
                Debug.LogWarning($"[DialogueCore] 未対応コマンド: {command}");
                break;
        }
    }

    // ===== #Give アイテム付与 =====
    private void CmdGiveItem(string args)
    {
        string itemId = (args ?? "").Trim();
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[DialogueCore] #Give 書式不正: itemId が空です");
            return;
        }

        var inv = InventoryManager.Instance;
        if (inv == null)
        {
            Debug.LogWarning("[DialogueCore] InventoryManager.Instance が見つかりません");
            return;
        }

        ItemData item = inv.allItems.Find(i => i != null && i.itemID == itemId);
        if (item == null)
        {
            Debug.LogWarning($"[DialogueCore] #Give 失敗: itemId='{itemId}' が見つかりません");
            return;
        }

        if (inv.HasItem(itemId))
        {
            Debug.Log($"[DialogueCore] #Give スキップ: 既に所持 itemId='{itemId}'");
            return;
        }

        inv.AddItem(item);
        Debug.Log($"[DialogueCore] #Give → AddItem 成功: itemId='{itemId}', itemName='{item.itemName}'");
    }

    // ===== ページ制御 =====
    private void EnsurePage(ref Page page, string speaker)
    {
        if (page == null) page = new Page { speaker = speaker };
    }

    private void AddLineAndSplitIfNeeded(ref Page page, string bodyLine)
    {
        page.lines.Add(bodyLine);
        if (page.lines.Count >= 3)
        {
            pages.Add(page);
            page = null;
        }
    }

    private void ShowPage(int index)
    {
        pageIndex = index;
        var p = pages[pageIndex];

        OnSpeakerChanged?.Invoke(p.speaker);
        OnPortraitChanged?.Invoke(p.speaker);
        OnLinesReady?.Invoke(p.lines.ToArray());
    }

    private void Finish(string id)
    {
        OnConversationEnded?.Invoke(id);
    }
}
