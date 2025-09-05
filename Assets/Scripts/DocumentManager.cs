using UnityEngine;
using TMPro;

public class DocumentManager : MonoBehaviour
{
    public static DocumentManager Instance;  // �V���O���g��

    [System.Serializable]
    public class DocumentData
    {
        public string title;           // �����^�C�g��
        [TextArea(3, 10)]
        public string body;            // �����{��
        [HideInInspector] public bool obtained = false; // ����ς݃t���O
    }

    [Header("�����f�[�^")]
    public DocumentData[] documents;   // Inspector�œo�^����i�{���͂����ɏ����j

    [Header("UI�Q��")]
    public GameObject documentGridPanel;   // �����ꗗ (DocumentGridPanel)
    public GameObject documentDetailPanel; // �����ڍ� (DocumentDetailPanel)
    public TMP_Text titleText;             // �ڍ׃^�C�g�� (TitleText)
    public TMP_Text bodyText;              // �ڍז{�� (BodyText)
    public DocumentSlot[] slots;           // �����X���b�g�Q�ƁiInspector�ŃZ�b�g�j

    public bool IsDetailOpen { get; private set; } = false; // �ڍ׉�ʂ��J���Ă��邩�ǂ���
    private int currentIndex = -1; // ���ݕ\�����Ă��鎑���ԍ�

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

    //  �V�K�ǉ��F�������菈��
    public void AddDocument(int id, string title)
    {
        if (id < 0 || id >= documents.Length) return;

        // ���ɓ���ς݂Ȃ牽�����Ȃ�
        if (documents[id].obtained) return;

        documents[id].obtained = true;
        documents[id].title = title; // �^�C�g�������X�V�i�{���� Inspector �ɓ��́j

        Debug.Log($"���� {title} ����肵�܂����I");
        RefreshUI();
    }

    //  UI�X�V�i�X���b�g�ɔ��f�j
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
    /// �����ꗗ���J��
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
    /// �����X���b�g���������Ƃ��ɌĂ�
    /// </summary>
    public void ShowDocument(int index)
    {
        if (index < 0 || index >= documents.Length) return;
        DocumentData data = documents[index];

        if (!data.obtained)
        {
            Debug.Log("���̃X���b�g�ɂ͎��������݂��܂���");
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
    /// �ڍ׉�ʂ���Ĉꗗ�ɖ߂�
    /// </summary>
    public void CloseDetail()
    {
        if (documentDetailPanel != null) documentDetailPanel.SetActive(false);
        if (documentGridPanel != null) documentGridPanel.SetActive(true);

        IsDetailOpen = false;
        currentIndex = -1;
    }
}