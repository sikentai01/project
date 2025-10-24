using UnityEngine;
using TMPro; // TextMeshProを使用するため
using System;

public class GimmickCanvasController : MonoBehaviour
{
    // ★ Button (2) - Button (3) - Button (4) - Button の Text (TMP) への参照を想定 ★
    [Header("ボタンテキストの参照")]
    [Tooltip("GimmickCanvas内の4つのボタンのText (TMP) コンポーネントを順に割り当ててください。")]
    public TMP_Text[] buttonLabels = new TMP_Text[4];

    [Header("会話表示用テキストパネル")]
    [Tooltip("DialogueCoreからテキストを受け取り表示するTMP_Text")]
    public TMP_Text conversationTextPanel;

    [Header("連動するボタンシーケンスギミック")]
    public ButtonSequenceGimmick sequenceGimmick; // ギミック本体への参照

    private static GimmickCanvasController instance;
    public static GimmickCanvasController Instance => instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // DialogueCoreのイベントに接続
        if (DialogueCore.Instance != null && conversationTextPanel != null)
        {
            // Coreがテキストを準備したときにテキストパネルを更新
            DialogueCore.Instance.OnLinesReady += SetConversationPanel;
            // 会話終了時にパネルをクリア (オプション)
            DialogueCore.Instance.OnConversationEnded += _ => conversationTextPanel.text = "";
        }
    }

    void OnDestroy()
    {
        if (DialogueCore.Instance != null && conversationTextPanel != null)
        {
            // 購読解除
            DialogueCore.Instance.OnLinesReady -= SetConversationPanel;
            DialogueCore.Instance.OnConversationEnded -= _ => conversationTextPanel.text = "";
        }
    }

    /// <summary>
    /// DialogueCoreから受け取った会話行をテキストパネルに表示する (流用機能)
    /// </summary>
    private void SetConversationPanel(string[] lines)
    {
        if (conversationTextPanel != null)
        {
            // 会話行を結合し、即座に表示
            conversationTextPanel.text = string.Join("\n", lines);
        }
    }


    /// <summary>
    /// 指定されたインデックスのボタンにテキストを設定する
    /// </summary>
    /// <param name="index">ボタンのインデックス (0～3)</param>
    /// <param name="text">設定するテキスト</param>
    public void SetButtonText(int index, string text)
    {
        if (buttonLabels.Length > index && buttonLabels[index] != null)
        {
            buttonLabels[index].text = text;
            Debug.Log($"[GimmickCanvas] ボタン {index + 1} にテキスト '{text}' を設定しました。");
        }
        else
        {
            Debug.LogWarning($"[GimmickCanvas] ボタンのインデックス {index} は無効か、参照がありません。");
        }
    }

    /// <summary>
    /// ボタンクリックイベントをギミックに転送する (Unityイベントから設定する)
    /// </summary>
    /// <param name="index">クリックされたボタンのインデックス (0～3)</param>
    public void OnAnyButtonClick(int index)
    {
        if (sequenceGimmick != null)
        {
            sequenceGimmick.OnButtonClick(index);
        }
        else
        {
            Debug.LogWarning("[GimmickCanvas] ButtonSequenceGimmickが設定されていません。");
        }
    }

    /// <summary>
    /// ギミックが完了した際にCanvasを非表示にする
    /// </summary>
    public void HideCanvas()
    {
        // ギミックが完了した場合、Canvas全体を非アクティブにする
        gameObject.SetActive(false);
        Debug.Log("[GimmickCanvas] Canvasを非表示にしました。");
    }
}