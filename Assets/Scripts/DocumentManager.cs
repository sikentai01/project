using UnityEngine;
using TMPro;

public class DocumentManager : MonoBehaviour
{
    // ===== �V���O���g�� =====
    public static DocumentManager Instance { get; private set; }

    [System.Serializable]
    public class DocumentData
    {
        public string title;    // �����^�C�g��
        [TextArea(3, 10)]
        public string body;     // �����{��
        public bool obtained;   // ����ς݂��ǂ���
    }

    [Header("�����f�[�^")]
    public DocumentData[] documents;   // Inspector�œo�^

    [Header("UI�Q��")]
    public GameObject documentGridPanel;   // �����ꗗ
    public GameObject documentDetailPanel; // �����ڍ�
    public TMP_Text titleText;             // �ڍ׃^�C�g��
    public TMP_Text bodyText;              // �ڍז{��
    public DocumentSlot[] slots;           // �X���b�g�z��

    public bool IsDetailOpen { get; private set; } = false;

    private int currentIndex = -1;

    // ===== Awake�ŃV���O���g�������� =====
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

    // ===== �����ǉ� =====
    public void AddDocument(int id, string title)
    {
        if (id < 0 || id >= documents.Length) return;

        if (documents[id].obtained) return;  // ���łɓ���ς݂Ȃ疳��

        documents[id].obtained = true;
        documents[id].title = title;

        Debug.Log($"�����u{title}�v����肵�܂����I");
        RefreshUI();
    }

    // ===== UI�X�V =====
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

    // ===== �ڍׂ�\�� =====
    public void ShowDocument(int index)
    {
        if (index < 0 || index >= documents.Length) return;
        DocumentData data = documents[index];

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