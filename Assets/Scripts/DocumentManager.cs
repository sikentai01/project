using UnityEngine;
using TMPro;

public class DocumentManager : MonoBehaviour
{
    [System.Serializable]
    public class DocumentData
    {
        public string title;           // 資料タイトル
        [TextArea(3, 10)]
        public string body;            // 資料本文
    }

    [Header("資料データ")]
    public DocumentData[] documents;   // Inspectorで登録する

    [Header("UI参照")]
    public GameObject documentGridPanel;   // 資料一覧 (DocumentGridPanel)
    public GameObject documentDetailPanel; // 資料詳細 (DocumentDetailPanel)
    public TMP_Text titleText;             // 詳細タイトル (TitleText)
    public TMP_Text bodyText;              // 詳細本文 (BodyText)

    public bool IsDetailOpen { get; private set; } = false; // 詳細画面が開いているかどうか

    private int currentIndex = -1; // 現在表示している資料番号

    void Start()
    {
        if (documentGridPanel != null) documentGridPanel.SetActive(false);
        if (documentDetailPanel != null) documentDetailPanel.SetActive(false);
    }

    /// <summary>
    /// 資料一覧を開く（PauseMenu から呼ぶ）
    /// </summary>
    public void OpenDocumentGrid()
    {
        if (documentGridPanel == null) return;

        documentGridPanel.SetActive(true);
        if (documentDetailPanel != null)
            documentDetailPanel.SetActive(false);

        IsDetailOpen = false;
        currentIndex = -1;
    }

    /// <summary>
    /// 資料スロットを押したときに呼ぶ
    /// </summary>
    public void ShowDocument(int index)
    {
        // 範囲外チェック
        if (index < 0 || index >= documents.Length) return;

        DocumentData data = documents[index];

        // 空スロットなら何もしない
        if (string.IsNullOrEmpty(data.title) || string.IsNullOrEmpty(data.body))
        {
            Debug.Log("このスロットには資料が存在しません");
            return;
        }

        currentIndex = index;

        // グリッドを閉じて詳細を開く
        if (documentGridPanel != null) documentGridPanel.SetActive(false);
        if (documentDetailPanel != null) documentDetailPanel.SetActive(true);

        // テキストを反映
        if (titleText != null) titleText.text = data.title;
        if (bodyText != null) bodyText.text = data.body;

        IsDetailOpen = true;
    }

    /// <summary>
    /// 詳細画面を閉じて一覧に戻る
    /// </summary>
    public void CloseDetail()
    {
        if (documentDetailPanel != null) documentDetailPanel.SetActive(false);
        if (documentGridPanel != null) documentGridPanel.SetActive(true);

        IsDetailOpen = false;
        currentIndex = -1;
    }
}
