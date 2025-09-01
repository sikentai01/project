using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;   // ���j���[�S��
    public GameObject charPanel;     // �L�����p�l��
    public GameObject itemPanel;     // �A�C�e���p�l��
    public GameObject documentPanel; // �����p�l�� (Grid��Detail�̐e)
    public GameObject optionPanel;   // �I�v�V�����p�l��
    public GameObject controlPanel;  // ��������p�l�� �� �V�����ǉ�
    public GameObject itemInfoPanel; // �A�C�e���o�[ (��̐�������)?

    [Header("Menu Buttons (���̃��j���[�p)")]
    public GameObject firstSelected;    // ESC�ōŏ��ɑI������{�^�� (Items)
    public GameObject documentButton;   // �����j���[�� Documents �{�^��
    public GameObject optionButton;     // �����j���[�� Options �{�^��
    public GameObject controlButton;    // �����j���[�� Controls �{�^��

    [Header("Panel First Buttons (�E�̃p�l���p)")]
    public GameObject firstItemButton;     // ItemPanel���ōŏ��ɑI������{�^��
    public GameObject firstDocumentButton; // DocumentPanel���ōŏ��ɑI������{�^��
    public GameObject firstOptionButton;   // OptionPanel���ōŏ��ɑI������{�^��

    [Header("Managers")]
    public DocumentManager documentManager; // Inspector�ŃZ�b�g����

    public static bool isPaused = false;
    private GameObject currentPanel = null;     // ���݊J���Ă���E���̃p�l��
    private GameObject lastMenuButton = null;   // ���O�ɊJ�������j���[�̃{�^��

    void Start()
    {
        pauseMenuUI.SetActive(false);
        charPanel.SetActive(false);
        itemPanel.SetActive(false);
        documentPanel.SetActive(false);
        optionPanel.SetActive(false);
        controlPanel.SetActive(false);
        if (itemInfoPanel != null) itemInfoPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                // DocumentDetail���J���Ă���ꍇ��Grid�ɖ߂�
                if (currentPanel == documentPanel && documentManager != null && documentManager.IsDetailOpen)
                {
                    documentManager.CloseDetail();
                }
                else if (currentPanel != null)
                {
                    // �A�C�e����ʂ̏ꍇ�̓o�[������
                    if (currentPanel == itemPanel && itemInfoPanel != null)
                        itemInfoPanel.SetActive(false);

                    currentPanel.SetActive(false);
                    currentPanel = null;
                    charPanel.SetActive(true);

                    // ESC�Ŗ߂�����u���O�̃{�^���v�ɃJ�[�\����߂�
                    if (lastMenuButton != null)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(lastMenuButton);
                    }
                }
                else
                {
                    Resume();
                }
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        charPanel.SetActive(false);
        itemPanel.SetActive(false);
        documentPanel.SetActive(false);
        optionPanel.SetActive(false);
        controlPanel.SetActive(false);
        if (itemInfoPanel != null) itemInfoPanel.SetActive(false);

        currentPanel = null;
        lastMenuButton = null; // Resume�����烊�Z�b�g

        Time.timeScale = 1f;
        isPaused = false;

        EventSystem.current.SetSelectedGameObject(null);
        Debug.Log("�Q�[���ĊJ");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        charPanel.SetActive(true);
        currentPanel = null;

        Time.timeScale = 0f;
        isPaused = true;

        // �J������Items��I��
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        Debug.Log("�|�[�Y���j���[���J����");
    }

    // �� �A�C�e���p�l��
    public void OpenItems()
    {
        charPanel.SetActive(false);
        if (currentPanel != null) currentPanel.SetActive(false);

        itemPanel.SetActive(true);
        if (itemInfoPanel != null) itemInfoPanel.SetActive(true);
        currentPanel = itemPanel;

        lastMenuButton = firstSelected;

        if (firstItemButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstItemButton);
        }

        Debug.Log("�A�C�e����ʂ��J����");
    }

    // �� �����p�l��
    public void OpenDocuments()
    {
        if (documentManager != null)
        {
            documentManager.OpenDocumentGrid(); // Grid���J��
        }

        documentPanel.SetActive(true); // �e�p�l�����\��
        currentPanel = documentPanel;

        lastMenuButton = documentButton;

        if (firstDocumentButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstDocumentButton);
        }

        Debug.Log("������ʂ��J����");
    }

    // �� �I�v�V�����p�l��
    public void OpenOptions()
    {
        SwitchPanel(optionPanel);
        lastMenuButton = optionButton;

        if (firstOptionButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstOptionButton);
        }

        Debug.Log("�I�v�V������ʂ��J����");
    }

    // �� ��������p�l��
    public void OpenControls()
    {
        SwitchPanel(controlPanel);
        lastMenuButton = controlButton;

        Debug.Log("����������J����");
    }

    void SwitchPanel(GameObject newPanel)
    {
        charPanel.SetActive(false);
        if (itemInfoPanel != null) itemInfoPanel.SetActive(false);

        if (currentPanel != null) currentPanel.SetActive(false);
        currentPanel = newPanel;
        currentPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("�Q�[���I��");
        Application.Quit();
    }
}
