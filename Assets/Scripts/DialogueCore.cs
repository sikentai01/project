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

    // === イベント ===
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
        if (textAsset == null)
        {
            Debug.LogWarning("[DialogueCore] TextAsset=null");
            Finish(id);
            return;
        }
        StartConversation(id, textAsset.text);
    }

    public void StartConversation(string id, string rawText)
    {
        currentConversationId = id;
        pages.Clear();
        pageIndex = -1;

        BuildPages(rawText);
        if (pages.Count == 0)
        {
            Finish(id);
            return;
        }
        ShowPage(0);
    }

    public void NextPage()
    {
        if (pages.Count == 0) return;
        int next = pageIndex + 1;
        if (next >= pages.Count)
        {
            Finish(currentConversationId);
            return;
        }
        ShowPage(next);
    }

    // ===== ページ構築 =====
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

            // 空行 = 改ページ
            if (string.IsNullOrEmpty(line))
            {
                FlushPageIfAny();
                continue;
            }

            // コマンド処理 (#Giveなど)
            if (IsCommand(line))
            {
                ExecuteCommand(line);
                continue;
            }

            // 「名前: 本文」形式
            if (TryParseSpeakerLine(line, out var maybeSpeaker, out var maybeBody))
            {
                if (!string.IsNullOrEmpty(currentSpeaker) && currentSpeaker != maybeSpeaker)
                    FlushPageIfAny();

                currentSpeaker = maybeSpeaker;

                if (!string.IsNullOrEmpty(maybeBody))
                {
                    EnsurePage(ref currentPage, currentSpeaker);
                    AddLineAndSplitIfNeeded(ref currentPage, maybeBody);
                }
                continue;
            }

            // 通常文（話者未設定ならナレーション扱い）
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
        s = s.Replace('\u3000', ' '); // 全角スペース→半角
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

    // ===== コマンド処理 (#Giveなど) =====
    private void ExecuteCommand(string line)
    {
        // #Give <itemID>
        if (line.StartsWith("#Give", StringComparison.OrdinalIgnoreCase))
        {
            var match = Regex.Match(line, @"^#Give\s+(\S+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string itemID = match.Groups[1].Value;

                // InventoryManagerに渡す
                if (InventoryManager.Instance != null)
                {
                    var found = InventoryManager.Instance.allItems.Find(i => i.itemID == itemID);
                    if (found != null)
                    {
                        InventoryManager.Instance.AddItem(found);
                        Debug.Log($"[DialogueCore] #Give → AddItem 成功: itemId='{itemID}', itemName='{found.itemName}'");
                    }
                    else
                    {
                        Debug.LogWarning($"[DialogueCore] #Give: itemID='{itemID}' に該当するアイテムが見つかりません。");
                    }
                }
                else
                {
                    Debug.LogWarning($"[DialogueCore] InventoryManager.Instance が存在しません (#Give {itemID})");
                }
            }
            return;
        }

        // 将来的に #Sound, #Effect など拡張予定ならここに追加
    }

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

    // ====== 🧩 追加: 現在ページをUIに再送信する ======
    public void PushCurrentToUI()
    {
        if (pages == null || pages.Count == 0 || pageIndex < 0 || pageIndex >= pages.Count)
            return;

        var p = pages[pageIndex];
        OnSpeakerChanged?.Invoke(p.speaker);
        OnPortraitChanged?.Invoke(p.speaker);
        OnLinesReady?.Invoke(p.lines.ToArray());
    }
}
