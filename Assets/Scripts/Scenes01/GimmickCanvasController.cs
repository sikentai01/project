using UnityEngine;
using UnityEngine.UI;

public class GimmickCanvasController : MonoBehaviour
{
    public static GimmickCanvasController Instance { get; private set; }

    [Header("キャンバス本体")]
    public Canvas canvasRoot;

    [Header("ボタン群（インスペクターで割り当て）")]
    public Button[] buttons;

    // 現在このキャンバスを使用中のギミック
    private ButtonSequenceGimmick activeGimmick;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvasRoot == null)
            canvasRoot = GetComponentInChildren<Canvas>();

        HideCanvas();
    }

    // ===============================
    //  基本表示制御
    // ===============================
    public void ShowCanvas(ButtonSequenceGimmick gimmick)
    {
        activeGimmick = gimmick;

        if (canvasRoot != null)
            canvasRoot.enabled = true;

        // ボタン押下時のリスナー設定
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() =>
            {
                OnButtonPressed(index);
            });
        }
    }

    public void HideCanvas()
    {
        if (canvasRoot != null)
            canvasRoot.enabled = false;

        activeGimmick = null;
    }

    // ===============================
    //  ボタン制御
    // ===============================
    private void OnButtonPressed(int index)
    {
        // 紐づいているギミックに通知
        if (activeGimmick != null)
        {
            activeGimmick.OnButtonClick(index);
        }
        else
        {
            Debug.LogWarning("[GimmickCanvasController] アクティブなギミックが未設定です。");
        }
    }

    /// <summary>
    /// 指定インデックスのボタンにテキストを設定
    /// </summary>
    public void SetButtonText(int index, string text)
    {
        if (buttons == null || index < 0 || index >= buttons.Length)
        {
            Debug.LogWarning($"[GimmickCanvasController] 無効なボタンインデックス: {index}");
            return;
        }

        var tmp = buttons[index].GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = text;
        else
            Debug.LogWarning($"[GimmickCanvasController] ボタン {index} に TextMeshProUGUI が見つかりません。");
    }

    /// <summary>
    /// すべてのボタンテキストを一括で設定
    /// </summary>
    public void SetAllButtonTexts(string[] texts)
    {
        if (buttons == null) return;

        for (int i = 0; i < buttons.Length && i < texts.Length; i++)
        {
            SetButtonText(i, texts[i]);
        }
    }
}
