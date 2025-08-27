using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;   // ���j���[�S��
    public GameObject charPanel;     // �L�����p�l��
    public GameObject itemPanel;     // �A�C�e���p�l��
    public GameObject documentPanel; // �����p�l��
    public GameObject optionPanel;   // �I�v�V�����p�l��
    public GameObject itemInfoPanel; // �A�C�e���o�[ (��̐�������)

    [Header("Menu Buttons (���̃��j���[�p)")]
    public GameObject firstSelected;    // ESC�ōŏ��ɑI������{�^�� (Items)
    public GameObject documentButton;   // �����j���[�� Documents �{�^��
    public GameObject optionButton;     // �����j���[�� Options �{�^��

    [Header("Panel First Buttons (�E�̃p�l���p)")]
    public GameObject firstItemButton;     // ItemPanel���ōŏ��ɑI������{�^�� (Item1)
    public GameObject firstDocumentButton; // DocumentPanel���ōŏ��ɑI������{�^��
    public GameObject firstOptionButton;   // OptionPanel���ōŏ��ɑI������{�^��

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
                // �E�p�l�����J���Ă���ꍇ�͕��ăL�����p�l���ɖ߂�
                if (currentPanel != null)
                {
                    currentPanel.SetActive(false);

                    // �A�C�e���̏ꍇ�̓o�[������
                    if (currentPanel == itemPanel && itemInfoPanel != null)
                        itemInfoPanel.SetActive(false);

                    currentPanel = null;
                    charPanel.SetActive(true);

                    //  ESC�Ŗ߂�����u���O�ɊJ���Ă����{�^���v�ɃJ�[�\����߂�
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
        if (itemInfoPanel != null) itemInfoPanel.SetActive(false);

        currentPanel = null;
        lastMenuButton = null; //  Resume�����烊�Z�b�g�i����ESC�ł�Items�Œ�j

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

        //  ���j���[���J�����Ƃ��͕K��Items�ɃJ�[�\����u��
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        Debug.Log("�|�[�Y���j���[���J����");
    }

    // �� �A�C�e���p�l�����J��
    public void OpenItems()
    {
        charPanel.SetActive(false);
        if (currentPanel != null) currentPanel.SetActive(false);

        itemPanel.SetActive(true);
        if (itemInfoPanel != null) itemInfoPanel.SetActive(true);
        currentPanel = itemPanel;

        lastMenuButton = firstSelected; //  ESC�߂���Items�{�^��

        // �A�C�e���p�l�����J������Item1�Ƀt�H�[�J�X
        if (firstItemButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstItemButton);
        }

        Debug.Log("�A�C�e����ʂ��J����");
    }

    // �� �����p�l�����J��
    public void OpenDocuments()
    {
        SwitchPanel(documentPanel);
        lastMenuButton = documentButton; //  ESC�߂���Documents�{�^��

        // �����p�l�����̍ŏ��̗v�f�Ƀt�H�[�J�X
        if (firstDocumentButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstDocumentButton);
        }

        Debug.Log("������ʂ��J����");
    }

    // �� �I�v�V�����p�l�����J��
    public void OpenOptions()
    {
        SwitchPanel(optionPanel);
        lastMenuButton = optionButton; //  ESC�߂���Options�{�^��

        // �I�v�V�����p�l�����̍ŏ��̗v�f�Ƀt�H�[�J�X
        if (firstOptionButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstOptionButton);
        }

        Debug.Log("�I�v�V������ʂ��J����");
    }

    // �� ���ʂ̐؂�ւ������i�A�C�e���ȊO�p�j
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
