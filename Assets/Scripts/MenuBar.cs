using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Linq;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance; // �V���O���g��

    [Header("�T�E���h�ݒ�")]
    [SerializeField] private AudioClip openSeClip;   // ���j���[���J��/���鎞
    [SerializeField] private AudioClip closeSeClip;  // ���j���[����鎞�i�I�v�V�����Ƃ��āj
    [SerializeField] private AudioClip selectSeClip; // ���j���[�I��/�p�l���؂�ւ���

    [Header("UI Panels")]
    public GameObject pauseMenuUI;
    public GameObject charPanel;
    public GameObject itemPanel;
    public GameObject documentPanel;
    public GameObject optionPanel;
    public GameObject controlPanel;
    public GameObject itemInfoPanel;

    [Header("Menu Buttons (���̃��j���[�p)")]
    public GameObject firstSelected;
    public GameObject documentButton;
    public GameObject optionButton;
    public GameObject controlButton;

    [Header("Panel First Buttons (�E�̃p�l���p)")]
    public GameObject firstItemButton;
    public GameObject firstDocumentButton;
    public GameObject firstOptionButton;

    [Header("Managers")]
    public DocumentManager documentManager;
    public OptionPanelManager optionPanelManager;

    public static bool isPaused = false;
    public static bool blockMenu = false; // ���C�x���g���� true �ɂ���

    private GameObject currentPanel = null;
    private GameObject lastMenuButton = null;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CloseAllPanels();
        pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        // --- ���j���[�𖳌�������V�[���Q ---
        string[] lockScenes = { "Title", "GameOver" };
        bool isMenuLocked = false;
        bool isPlayableSceneLoaded = false;

        int sceneCount = SceneManager.sceneCount;

        for (int i = 0; i < sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);

            // Title��GameOver��1�ł����[�h����Ă����疳�����Ń��j���[����
            if (lockScenes.Contains(s.name))
            {
                isMenuLocked = true;
                break;
            }

            // �Q�[���V�[���iScene0�Ƃ��j�����邩���m�F
            if (s.name.StartsWith("Scene"))
            {
                isPlayableSceneLoaded = true;
            }
        }

        // Title/GameOver������ΐ�΂ɊJ���Ȃ�
        if (isMenuLocked)
            return;

        // Scene�n�������ꍇ�i�Q�[���O�j���J���Ȃ�
        if (!isPlayableSceneLoaded)
            return;

        if (PauseMenu.blockMenu)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                if (currentPanel == documentPanel && documentManager != null && documentManager.IsDetailOpen)
                {
                    documentManager.CloseDetail();
                }
                else if (currentPanel != null)
                {
                    if (SoundManager.Instance != null && closeSeClip != null)
                    {
                        SoundManager.Instance.PlaySE(closeSeClip);
                    }
                    if (currentPanel == itemPanel && itemInfoPanel != null)
                        itemInfoPanel.SetActive(false);

                    currentPanel.SetActive(false);
                    currentPanel = null;
                    charPanel.SetActive(true);

                    if (lastMenuButton != null)
                    {
                        EventSystem.current.SetSelectedGameObject(null);
                        EventSystem.current.SetSelectedGameObject(lastMenuButton);
                    }
                }
                else
                {
                    if (SoundManager.Instance != null && closeSeClip != null)
                    {
                        SoundManager.Instance.PlaySE(closeSeClip);
                    }
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
        CloseAllPanels();
        pauseMenuUI.SetActive(false);

        currentPanel = null;
        lastMenuButton = null;

        Time.timeScale = 1f;
        isPaused = false;

        EventSystem.current.SetSelectedGameObject(null);
        Debug.Log("�Q�[���ĊJ");
    }

    void Pause()
    {
        if (SoundManager.Instance != null && openSeClip != null)
        {
            SoundManager.Instance.PlaySE(openSeClip);
        }

        pauseMenuUI.SetActive(true);
        charPanel.SetActive(true);
        currentPanel = null;

        Time.timeScale = 0f;
        isPaused = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        Debug.Log("�|�[�Y���j���[���J����");
    }

    public void OpenItems()
    {
        if (SoundManager.Instance != null && selectSeClip != null)
        {
            SoundManager.Instance.PlaySE(selectSeClip);
        }
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

            var slot = firstItemButton.GetComponent<ItemSlot>();
            if (slot != null) slot.OnSelectSlot();
        }

        Debug.Log("�A�C�e����ʂ��J����");
    }

    public void OpenDocuments()
    {
        if (SoundManager.Instance != null && selectSeClip != null)
        {
            SoundManager.Instance.PlaySE(selectSeClip);
        }
        charPanel.SetActive(false);

        if (documentManager != null)
        {
            documentManager.OpenDocumentGrid();
        }

        documentPanel.SetActive(true);
        currentPanel = documentPanel;

        lastMenuButton = documentButton;

        if (firstDocumentButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstDocumentButton);
        }

        Debug.Log("������ʂ��J����");
    }

    public void OpenOptions()
    {
        if (SoundManager.Instance != null && selectSeClip != null)
        {
            SoundManager.Instance.PlaySE(selectSeClip);
        }
        SwitchPanel(optionPanel);
        lastMenuButton = optionButton;

        if (optionPanelManager != null)
        {
            optionPanelManager.InitializeSliders();
        }
        if (firstOptionButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstOptionButton);
        }

        Debug.Log("�I�v�V������ʂ��J����");
    }

    public void OpenControls()
    {
        if (SoundManager.Instance != null && selectSeClip != null)
        {
            SoundManager.Instance.PlaySE(selectSeClip);
        }
        charPanel.SetActive(false);
        if (currentPanel != null) currentPanel.SetActive(false);

        controlPanel.SetActive(true);
        currentPanel = controlPanel;

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

    public void CloseAllPanels()
    {
        pauseMenuUI.SetActive(false);
        charPanel.SetActive(false);
        itemPanel.SetActive(false);
        documentPanel.SetActive(false);
        optionPanel.SetActive(false);
        controlPanel.SetActive(false);
        if (itemInfoPanel != null) itemInfoPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("�^�C�g���ɖ߂�");

        Time.timeScale = 1f;
        CloseAllPanels();
        pauseMenuUI.SetActive(false);
        isPaused = false;

        // Additive�\�����l�����āARoomLoader�o�R�Ń��[�h
        RoomLoader.LoadRoom("Title", null);
    }


}