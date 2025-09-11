using UnityEngine;
using TMPro;

public class DocumentManager : MonoBehaviour
{
    public static DocumentManager Instance { get; private set; }

    [System.Serializable]
    public class DocumentData
    {
        public string documentID;   // ���j�[�NID
        public string title;        // �^�C�g��
        [TextArea(3, 10)]
        public string body;         // �{��
        public bool obtained;       // ����ς݂��ǂ����iInspector�Őݒ� or �}�b�v�œ���j
    }

    [Header("�����f�[�^")]
    public DocumentData[] documents;   // Inspector�œo�^

    [Header("UI�Q��")]
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

        //Debug.Log("�o�^����Ă���X���b�g��: " + slots.Length);
        //for (int i = 0; i < slots.Length; i++)
        //{
        //    Debug.Log($"Slot {i}: " + slots[i]);
        //}

        if (documentDetailPanel != null)
            documentDetailPanel.SetActive(false);

        RefreshUI();
    }

    // ===== �������� =====
    public void AddDocument(string id, string title)
    {
        var doc = System.Array.Find(documents, d => d.documentID == id);
        if (doc == null)
        {
            Debug.LogWarning($"ID {id} �̎�����������܂���");
            return;
        }

        if (doc.obtained) return; // ���ɓ���ς݂Ȃ疳��

        doc.obtained = true;
        doc.title = title;

        Debug.Log($"�����u{title}�v����肵�܂����I");
        RefreshUI();
    }

    // ===== UI�X�V =====
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

    // ===== �ꗗ���J�� =====
    public void OpenDocumentGrid()
    {
        if (documentGridPanel == null) return;

        documentGridPanel.SetActive(true);
        if (documentDetailPanel != null)
            documentDetailPanel.SetActive(false);

        IsDetailOpen = false;
        currentIndex = -1;
    }

    // ===== �ڍו\�� =====
    public void ShowDocument(int index)
    {
        if (index < 0 || index >= documents.Length) return;
        var data = documents[index];

        if (!data.obtained)
        {
            Debug.Log("���̎����͖�����ł�");
            return;
        }

        currentIndex = index;

        if (documentGridPanel != null) documentGridPanel.SetActive(false);
        if (documentDetailPanel != null) documentDetailPanel.SetActive(true);

        if (titleText != null) titleText.text = data.title;
        if (bodyText != null) bodyText.text = data.body;

        IsDetailOpen = true;
    }

    // ===== �ڍׂ���� =====
    public void CloseDetail()
    {
        if (documentDetailPanel != null) documentDetailPanel.SetActive(false);
        if (documentGridPanel != null) documentGridPanel.SetActive(true);

        IsDetailOpen = false;
        currentIndex = -1;
    }
}