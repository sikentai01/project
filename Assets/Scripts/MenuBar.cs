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

    [Header("Menu Buttons")]
    public GameObject firstSelected;    // ESC�ōŏ��ɑI������{�^�� (Items)
    public GameObject firstItemButton;  // ItemPanel���ōŏ��ɑI������{�^�� (Item1)

    public static bool isPaused = false;
    private GameObject currentPanel = null; // ���݊J���Ă���E���̃p�l��

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
                // �����E�p�l�����J���Ă�Ȃ���ăL�����p�l���ɖ߂�
                if (currentPanel != null)
                {
                    currentPanel.SetActive(false);
                    currentPanel = null;
                    charPanel.SetActive(true);

                    // ESC�Ŗ߂�����J�[�\�����uItems�v�ɖ߂�
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(firstSelected);
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

        Time.timeScale = 1f;
        isPaused = false;

        // �I������
        EventSystem.current.SetSelectedGameObject(null);

        Debug.Log("�Q�[���ĊJ");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        charPanel.SetActive(true); // �ŏ��̓L�����p�l���\��
        currentPanel = null;

        Time.timeScale = 0f;
        isPaused = true;

        //  ���j���[���J�������́uItems�v�{�^���ɃJ�[�\����u��
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
        if (itemInfoPanel != null) itemInfoPanel.SetActive(true); // �A�C�e���o�[���\��
        currentPanel = itemPanel;

        //  �A�C�e���p�l�����J�������́uItem1�v�ɃJ�[�\�������킹��
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
        Debug.Log("������ʂ��J����");
    }

    // �� �I�v�V�����p�l�����J��
    public void OpenOptions()
    {
        SwitchPanel(optionPanel);
        Debug.Log("�I�v�V������ʂ��J����");
    }

    // �� ���ʂ̐؂�ւ�����
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
