using UnityEngine;
using TMPro;

public class DocumentManager : MonoBehaviour
{
    [System.Serializable]
    public class DocumentData
    {
        public string title;           // �����^�C�g��
        [TextArea(3, 10)]
        public string body;            // �����{��
    }

    [Header("�����f�[�^")]
    public DocumentData[] documents;   // Inspector�œo�^����

    [Header("UI�Q��")]
    public GameObject documentGridPanel;   // �����ꗗ (DocumentGridPanel)
    public GameObject documentDetailPanel; // �����ڍ� (DocumentDetailPanel)
    public TMP_Text titleText;             // �ڍ׃^�C�g�� (TitleText)
    public TMP_Text bodyText;              // �ڍז{�� (BodyText)

    public bool IsDetailOpen { get; private set; } = false; // �ڍ׉�ʂ��J���Ă��邩�ǂ���

    private int currentIndex = -1; // ���ݕ\�����Ă��鎑���ԍ�

    void Start()
    {
        if (documentGridPanel != null) documentGridPanel.SetActive(false);
        if (documentDetailPanel != null) documentDetailPanel.SetActive(false);
    }

    /// <summary>
    /// �����ꗗ���J���iPauseMenu ����Ăԁj
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
    /// �����X���b�g���������Ƃ��ɌĂ�
    /// </summary>
    public void ShowDocument(int index)
    {
        // �͈͊O�`�F�b�N
        if (index < 0 || index >= documents.Length) return;

        DocumentData data = documents[index];

        // ��X���b�g�Ȃ牽�����Ȃ�
        if (string.IsNullOrEmpty(data.title) || string.IsNullOrEmpty(data.body))
        {
            Debug.Log("���̃X���b�g�ɂ͎��������݂��܂���");
            return;
        }

        currentIndex = index;

        // �O���b�h����ďڍׂ��J��
        if (documentGridPanel != null) documentGridPanel.SetActive(false);
        if (documentDetailPanel != null) documentDetailPanel.SetActive(true);

        // �e�L�X�g�𔽉f
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
