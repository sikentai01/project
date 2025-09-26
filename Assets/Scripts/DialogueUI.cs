using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [SerializeField] DialogueCore core;   // ← ここに DialogueCoreRoot をドラッグ
    [SerializeField] TMP_Text charaText;  // ← CharaText (TMP)
    [SerializeField] TMP_Text bodyText;   // ← 本文 Text (TMP)

    void Awake()
    {
        if (!core) core = GetComponentInParent<DialogueCore>(); // 親から自動取得の保険

        // Coreのイベントを購読
        if (core)
        {
            core.OnSpeakerChanged += s => { if (charaText) charaText.text = s; };
            core.OnLinesReady     += lines =>
            {
                if (bodyText) bodyText.text = string.Join("\n", lines);
            };
            // 立ち絵やコマンドをここで受けたいなら、同様に購読できます:
            // core.OnPortraitChanged += key => ...
            // core.OnCommand        += cmd => ...
        }
        else
        {
            Debug.LogError("[DialogueUI] Core が見つかりません。Core参照を割り当ててください。");
        }
    }
}
