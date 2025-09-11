using UnityEngine;
using TMPro;

public class DocumentManager : MonoBehaviour
{
    public static DocumentManager Instance { get; private set; }

    [System.Serializable]
    public class DocumentData
    {
        public string documentID;   // ユニークID
        public string title;        // タイトル
        [TextArea(3, 10)]
        public string body;         // 本文
        public bool obtained;       // 入手済みかどうか（Inspectorで設定 or マップで入手）
    }

    [Header("資料データ")]
    public DocumentData[] documents;   // Inspectorで登録

    [Header("UI参照")]
    public GameObject documentGridPanel;
    public GameObject documentDetailPanel;
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public DocumentSlot[] slots;

    public bool IsDetailOpen { get; private set; } = false;

    private int currentIndex = -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (slots == null || slots.Length == 0)
        {
            slots = GetComponentsInChildren<DocumentSlot>(true);
        }

        //Debug.Log("登録されているスロット数: " + slots.Length);
        //for (int i = 0; i < slots.Length; i++)
        //{
        //    Debug.Log($"Slot {i}: " + slots[i]);
        //}

        if (documentDetailPanel != null)
            documentDetailPanel.SetActive(false);

        RefreshUI();
    }

    // ===== 資料入手 =====
    public void AddDocument(string id, string title)
    {
        var doc = System.Array.Find(documents, d => d.documentID == id);
        if (doc == null)
        {
            Debug.LogWarning($"ID {id} の資料が見つかりません");
            return;
        }

        if (doc.obtained) return; // 既に入手済みなら無視

        doc.obtained = true;
        doc.title = title;

        Debug.Log($"資料「{title}」を入手しました！");
        RefreshUI();
    }

    // ===== UI更新 =====
    void RefreshUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < documents.Length && documents[i].obtained)
                slots[i].SetDocument(documents[i]);
            else
                slots[i].ClearSlot();
        }
    }

    // ===== 一覧を開く =====
    public void OpenDocumentGrid()
    {
        if (documentGridPanel == null) return;

        documentGridPanel.SetActive(true);
        if (documentDetailPanel != null)
            documentDetailPanel.SetActive(false);

        IsDetailOpen = false;
        currentIndex = -1;
    }

    // ===== 詳細表示 =====
    public void ShowDocument(int index)
    {
        if (index < 0 || index >= documents.Length) return;
        var data = documents[index];

        if (!data.obtained)
        {
            Debug.Log("この資料は未入手です");
            return;
        }

        currentIndex = index;

        if (documentGridPanel != null) documentGridPanel.SetActive(false);
        if (documentDetailPanel != null) documentDetailPanel.SetActive(true);

        if (titleText != null) titleText.text = data.title;
        if (bodyText != null) bodyText.text = data.body;

        IsDetailOpen = true;
    }

    // ===== 詳細を閉じる =====
    public void CloseDetail()
    {
        if (documentDetailPanel != null) documentDetailPanel.SetActive(false);
        if (documentGridPanel != null) documentGridPanel.SetActive(true);

        IsDetailOpen = false;
        currentIndex = -1;
    }
}