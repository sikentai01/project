using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class DialogueCore : MonoBehaviour
{
    [Header("Source")]
    [SerializeField] string fileName = "sample.txt";              // StreamingAssets/Dialogue 内
    [Header("Flow")]
    [SerializeField] int   linesPerPage = 3;
    [SerializeField] KeyCode nextKey    = KeyCode.Space;

    // 外部通知
    public event Action<string>   OnSpeakerChanged;
    public event Action<string>   OnPortraitChanged;
    public event Action<string[]> OnLinesReady;
    public event Action<string>   OnCommand;

    string[] allLines = Array.Empty<string>();
    int cursor = 0;

    // Regex（すべて string を返す Value を使用）
    static readonly Regex RxCmd      = new Regex(@"^\s*#\s*command\s*(?:=\s*(.*))?\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    static readonly Regex RxChara    = new Regex(@"^\s*#\s*chara\s*=\s*(.+)\s*$",        RegexOptions.IgnoreCase | RegexOptions.Compiled);
    static readonly Regex RxPortrait = new Regex(@"^\s*#\s*portrait\s*=\s*(.+)\s*$",     RegexOptions.IgnoreCase | RegexOptions.Compiled);
    static readonly Regex RxBracket  = new Regex(@"^\s*\[(.+?)\]\s*(.*)$",                RegexOptions.Compiled); // [名前] 文章
    static readonly Regex RxNamed    = new Regex(@"^\s*([^:：]+)\s*[:：]\s*(.+)$",         RegexOptions.Compiled); // 名前: 文章

    void Start()
    {
        LoadFile(fileName);
        ShowNext();
    }

    void Update()
    {
        if (Input.GetKeyDown(nextKey)) ShowNext();
    }

    public void SetFile(string newFileName, bool showFirstImmediately = true)
    {
        LoadFile(newFileName);
        if (showFirstImmediately) ShowNext();
    }

    public void ShowNext()
    {
        if (allLines.Length == 0)
        {
            OnLinesReady?.Invoke(Array.Empty<string>());
            return;
        }

        var page  = new List<string>();
        int shown = 0;
        int max   = Mathf.Max(1, linesPerPage);

        while (cursor < allLines.Length && shown < max)
        {
            string raw = allLines[cursor].TrimEnd();
            cursor++;

            if (string.IsNullOrWhiteSpace(raw)) continue;

            // ---- タグ（表示行にカウントしない）----
            Match m; // ← var ではなく Match で宣言

            if ((m = RxCmd.Match(raw)).Success)
            {
                string payload = (m.Groups.Count > 1 ? m.Groups[1].Value : "").Trim(); // Value を使う
                OnCommand?.Invoke(payload);
                continue;
            }

            if ((m = RxChara.Match(raw)).Success)
            {
                string name = m.Groups[1].Value.Trim();
                OnSpeakerChanged?.Invoke(name);
                continue;
            }

            if ((m = RxPortrait.Match(raw)).Success)
            {
                string key = m.Groups[1].Value.Trim();
                OnPortraitChanged?.Invoke(key);
                continue;
            }

            // ---- [名前] 文章 ----
            if ((m = RxBracket.Match(raw)).Success)
            {
                string name = m.Groups[1].Value.Trim();
                string text = m.Groups[2].Value;              // ← Value
                if (!string.IsNullOrEmpty(name)) OnSpeakerChanged?.Invoke(name);
                if (!string.IsNullOrWhiteSpace(text)) { page.Add(text); shown++; }
                continue;
            }

            // ---- 名前: 文章 ----
            if ((m = RxNamed.Match(raw)).Success)
            {
                string name = m.Groups[1].Value.Trim();
                string text = m.Groups[2].Value;
                if (!string.IsNullOrEmpty(name)) OnSpeakerChanged?.Invoke(name);
                page.Add(text);
                shown++;
                continue;
            }

            // ---- 通常行 ----
            page.Add(raw);
            shown++;
        }

        OnLinesReady?.Invoke(page.ToArray());
    }

    void LoadFile(string fname)
    {
        cursor = 0;
        string use = string.IsNullOrEmpty(fname) ? fileName : fname;
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogue", use);

        allLines = File.Exists(path) ? File.ReadAllLines(path) : Array.Empty<string>();
        if (allLines.Length == 0)
            Debug.LogWarning($"[DialogueCore] File missing or empty: {path}");
    }
}
