using UnityEngine;
using TMPro;

public class DocumentManager : MonoBehaviour
{
    public static DocumentManager Instance;  // シングルトン

    [System.Serializable]
    public class DocumentData
    {
        public string title;           // 資料タイトル
        [TextArea(3, 10)]
        public string body;            // 資料本文
        [HideInInspector] public bool obtained = false; // 入手済みフラグ
    }

    [Header("資料データ")]
    public DocumentData[] documents;   // Inspectorで登録する（本文はここに書く）

    [Header("UI参照")]
    public GameObject documentGridPanel;   // 資料一覧 (DocumentGridPanel)
    public GameObject documentDetailPanel; // 資料詳細 (DocumentDetailPanel)
    public TMP_Text titleText;             // 詳細タイトル (TitleText)
    public TMP_Text bodyText;              // 詳細本文 (BodyText)
    public DocumentSlot[] slots;           // 資料スロット参照（Inspectorでセット）

    public bool IsDetailOpen { get; private set; } = false; // 詳細画面が開いているかどうか
    private int currentIndex = -1; // 現在表示している資料番号

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (documentDetailPanel != null) documentDetailPanel.SetActive(false);
        RefreshUI();
    }

    //  新規追加：資料入手処理
    public void AddDocument(int id, string title)
    {
        if (id < 0 || id >= documents.Length) return;

        // 既に入手済みなら何もしない
        if (documents[id].obtained) return;

        documents[id].obtained = true;
        documents[id].title = title; // タイトルだけ更新（本文は Inspector に入力）

        Debug.Log($"資料 {title} を入手しました！");
        RefreshUI();
    }

    //  UI更新（スロットに反映）
    void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < documents.Length && documents[i].obtained)
            {
                slots[i].SetDocument(documents[i]);
            }
            else
            {
                slots[i].ClearSlot();
            }
        }
    }

    /// <summary>
    /// 資料一覧を開く
    /// </summary>
    public void OpenDocumentGrid()
    {
        if (documentGridPanel == null) return;

        documentGridPanel.SetActive(true);
        if (documentDetailPanel != null)
            documentDetailPanel.SetActive(false);

        IsDetailOpen = false;
        currentIndex = -1;

        RefreshUI();
    }

    /// <summary>
    /// 資料スロットを押したときに呼ぶ
    /// </summary>
    public void ShowDocument(int index)
    {
        if (index < 0 || index >= documents.Length) return;
        DocumentData data = documents[index];

        if (!data.obtained)
        {
            Debug.Log("このスロットには資料が存在しません");
            return;
        }

        currentIndex = index;

        if (documentGridPanel != null) documentGridPanel.SetActive(false);
        if (documentDetailPanel != null) documentDetailPanel.SetActive(true);

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