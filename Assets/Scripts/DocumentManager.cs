using UnityEngine;
using TMPro;

public class DocumentManager : MonoBehaviour
{
    // ===== シングルトン =====
    public static DocumentManager Instance { get; private set; }

    [System.Serializable]
    public class DocumentData
    {
        public string title;    // 資料タイトル
        [TextArea(3, 10)]
        public string body;     // 資料本文
        public bool obtained;   // 入手済みかどうか
    }

    [Header("資料データ")]
    public DocumentData[] documents;   // Inspectorで登録

    [Header("UI参照")]
    public GameObject documentGridPanel;   // 資料一覧
    public GameObject documentDetailPanel; // 資料詳細
    public TMP_Text titleText;             // 詳細タイトル
    public TMP_Text bodyText;              // 詳細本文
    public DocumentSlot[] slots;           // スロット配列

    public bool IsDetailOpen { get; private set; } = false;

    private int currentIndex = -1;

    // ===== Awakeでシングルトン初期化 =====
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (documentDetailPanel != null)
            documentDetailPanel.SetActive(false);

        RefreshUI();
    }

    // ===== 資料追加 =====
    public void AddDocument(int id, string title)
    {
        if (id < 0 || id >= documents.Length) return;

        if (documents[id].obtained) return;  // すでに入手済みなら無視

        documents[id].obtained = true;
        documents[id].title = title;

        Debug.Log($"資料「{title}」を入手しました！");
        RefreshUI();
    }

    // ===== UI更新 =====
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

    // ===== 詳細を表示 =====
    public void ShowDocument(int index)
    {
        if (index < 0 || index >= documents.Length) return;
        DocumentData data = documents[index];

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