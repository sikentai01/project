using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] DialogueCore core;
    [SerializeField] TMP_Text charaText;
    [SerializeField] TMP_Text bodyText;
    [SerializeField] float typeSpeed = 0.03f;

    Coroutine typingCoroutine;
    bool isTyping, skipRequested;

    void Awake()
    {
        if (!core) core = GetComponentInParent<DialogueCore>(true);

        if (core)
        {
            // イベント購読設定
            core.OnSpeakerChanged += s =>
            {
                if (charaText) charaText.text = s;
            };

            core.OnLinesReady += lines =>
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                isTyping = false;
                skipRequested = false;
                typingCoroutine = StartCoroutine(TypeLines(lines ?? System.Array.Empty<string>()));
            };

            // ★追加：最初の会話開始直後に名前が出ない対策
            StartCoroutine(DelayedSync());
        }
        else
        {
            Debug.LogError("[DialogueUI] Core が見つかりません。");
        }
    }

    // ===== 最初のページを反映（UI同期用） =====
    private IEnumerator DelayedSync()
    {
        yield return null; // フレームを1つ待ってCore初期化を待機
        if (core != null)
            core.PushCurrentToUI(); // ← Core側のヘルパーを呼ぶ
    }

    void Update()
    {
        if (isTyping && Input.GetKeyDown(KeyCode.Space))
            skipRequested = true;
    }

    IEnumerator TypeLines(string[] lines)
    {
        if (!bodyText) yield break;

        bodyText.text = "";
        isTyping = true;
        skipRequested = false;

        string fixedPrefix = ""; // ← 既に確定した行
        foreach (var line in lines)
        {
            string current = ""; // ← 現在タイプ中の行

            string prefixPlus = string.IsNullOrEmpty(fixedPrefix) ? "" : fixedPrefix + "\n";

            foreach (char c in line)
            {
                if (skipRequested)
                {
                    current = line;
                    break;
                }

                current += c;
                bodyText.text = prefixPlus + current;
                yield return new WaitForSeconds(typeSpeed);
            }

            // 行確定
            fixedPrefix = string.IsNullOrEmpty(fixedPrefix)
                ? line
                : fixedPrefix + "\n" + line;

            bodyText.text = fixedPrefix;
            skipRequested = false;
        }

        isTyping = false;
    }
}
