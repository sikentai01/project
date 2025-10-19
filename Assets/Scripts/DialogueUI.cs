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
            core.OnSpeakerChanged += s => { if (charaText) charaText.text = s; };
            core.OnLinesReady += lines =>
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                isTyping = false; skipRequested = false;
                typingCoroutine = StartCoroutine(TypeLines(lines));
            };
        }
        else
        {
            Debug.LogError("[DialogueUI] Core が見つかりません。");
        }
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

        string fixedPrefix = ""; // ← 確定済みの前行たち

        foreach (var line in lines)
        {
            string current = ""; // ← いま打っている行

            // 毎フレーム「prefix + 現在の部分」だけを描く
            string prefixPlus = string.IsNullOrEmpty(fixedPrefix) ? "" : fixedPrefix + "\n";

            foreach (char c in line)
            {
                if (skipRequested) { current = line; break; }

                current += c;
                bodyText.text = prefixPlus + current; // ← 前行は固定、今行だけ伸ばす
                yield return new WaitForSeconds(typeSpeed);
            }

            // 行を確定して prefix に吸収
            fixedPrefix = string.IsNullOrEmpty(fixedPrefix) ? line : fixedPrefix + "\n" + line;
            bodyText.text = fixedPrefix;

            skipRequested = false; // 次の行はまたタイプする
        }

        isTyping = false;
    }
}
