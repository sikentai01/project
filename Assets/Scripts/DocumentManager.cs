using UnityEngine;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class DocumentSaveData
{
    public List<string> obtainedIDs = new List<string>();
}

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
        public bool obtained;       // 入手済みかどうか
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
            Debug.LogWarning($"[DocumentManager] ID {id} の資料が見つかりません");
            return;
        }

        if (doc.obtained) return; // 既に入手済みなら無視

        doc.obtained = true;
        doc.title = title;

        Debug.Log($"[DocumentManager] 資料「{title}」を入手しました！");
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

    // ===== セーブデータ作成 =====
    public DocumentSaveData SaveData()
    {
        var data = new DocumentSaveData();

        foreach (var doc in documents)
        {
            if (doc.obtained)
                data.obtainedIDs.Add(doc.documentID);
        }

        Debug.Log($"[DocumentManager] {data.obtainedIDs.Count} 件の資料を保存しました");
        return data;
    }

    // ===== セーブデータ読み込み =====
    public void LoadData(DocumentSaveData data)
    {
        foreach (var doc in documents)
        {
            doc.obtained = data.obtainedIDs.Contains(doc.documentID);
        }

        RefreshUI();
        Debug.Log("[DocumentManager] 資料データをロードしました");
    }

    // ===== 初期化 =====
    public void ClearAll()
    {
        foreach (var doc in documents)
        {
            doc.obtained = false;
        }

        RefreshUI();
        Debug.Log("[DocumentManager] 全資料を初期化しました");
    }

}
