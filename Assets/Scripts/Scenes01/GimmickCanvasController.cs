using UnityEngine;
using TMPro; // TextMeshProを使用するため
using System;

public class GimmickCanvasController : MonoBehaviour
{
    // ★ Button (2) - Button (3) - Button (4) - Button の Text (TMP) への参照を想定 ★
    [Header("ボタンテキストの参照")]
    public TMP_Text[] buttonLabels = new TMP_Text[4];

    [Header("中央テキストパネル")]
    public TMP_Text centerTextPanel; // ★ 会話パネルの代わりに中央パネルとして使用

    [Header("連動するボタンシーケンスギミック")]
    public ButtonSequenceGimmick sequenceGimmick;

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
    }

    /// <summary>
    /// 中央テキストパネルにメッセージを設定する（会話システム流用なし）
    /// </summary>
    public void SetCenterMessage(string message)
    {
        if (centerTextPanel != null)
        {
            centerTextPanel.text = message;
        }
    }

    /// <summary>
    /// 指定されたインデックスのボタンにテキストを設定する
    /// </summary>
    public void SetButtonText(int index, string text)
    {
        if (buttonLabels.Length > index && buttonLabels[index] != null)
        {
            buttonLabels[index].text = text;
        }
    }
    // ... (OnAnyButtonClick, HideCanvas はそのまま) ...

    /// <summary>
    /// ボタンクリックイベントをギミックに転送する (Unityイベントから設定する)
    /// </summary>
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

    public void HideCanvas()
    {
        gameObject.SetActive(false);
    }
}