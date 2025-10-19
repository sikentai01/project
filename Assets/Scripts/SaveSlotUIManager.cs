using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SaveSlotUIManager : MonoBehaviour
{
    public static SaveSlotUIManager Instance;

    [Header("UI�Q��")]
    [SerializeField] private GameObject saveSlotCanvas;

    private bool isLoadMode = false;
    private bool isOpen = false;
    private bool recentlyClosed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (saveSlotCanvas != null)
            saveSlotCanvas.SetActive(false);
    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    // ======================
    // �Z�[�u���[�h�ŊJ��
    // ======================
    public void OpenSavePanel()
    {
        isLoadMode = false;
        OpenPanelInternal();
    }

    // ======================
    // ���[�h���[�h�ŊJ��
    // ======================
    public void OpenLoadPanel()
    {
        isLoadMode = true;
        OpenPanelInternal();
    }

    // ======================
    // ���ʂ̊J������
    // ======================
    private void OpenPanelInternal()
    {
        if (saveSlotCanvas == null) return;

        saveSlotCanvas.SetActive(true);
        isOpen = true;

        var buttons = saveSlotCanvas.GetComponentsInChildren<SaveSlotButton>(true);
        foreach (var btn in buttons)
            btn.SetMode(isLoadMode);

        // --- �ŏ��̃X���b�g��I�� ---
        if (buttons.Length > 0 && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        }

        if (PauseMenu.Instance != null)
            PauseMenu.blockMenu = true;

        Debug.Log($"[SaveSlotUIManager] {(isLoadMode ? "���[�h" : "�Z�[�u")}�X���b�g���J���܂����B");
    }

    // ======================
    // ���鏈��
    // ======================
    public void ClosePanel()
    {
        if (saveSlotCanvas == null) return;

        saveSlotCanvas.SetActive(false);
        isOpen = false;

        // --- ���j���[���ėL���� ---
        if (PauseMenu.Instance != null)
        {
            PauseMenu.blockMenu = false;
            PauseMenu.isPaused = false;
            Time.timeScale = 1f;
        }

        // --- �Q�[�����̓v���C���[�ėL���� ---
        var player = FindFirstObjectByType<GridMovement>();
        if (player != null)
            player.enabled = true;

        // --- �^�C�g����ʂŃ��[�h�����Ƃ� ---
        if (isLoadMode && EventSystem.current != null)
        {
            var continueButton = GameObject.Find("ContinueButton"); // �^�C�g���V�[����̖��O�ɍ��킹��
            if (continueButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(continueButton);
                Debug.Log("[SaveSlotUIManager] �J�[�\���𑱂�����ɖ߂��܂����B");
            }
        }

        recentlyClosed = true;
        StartCoroutine(ResetRecentlyClosed());

        Debug.Log("[SaveSlotUIManager] �X���b�gUI����܂����B");
    }

    private IEnumerator ResetRecentlyClosed()
    {
        yield return null;
        recentlyClosed = false;
    }

    public bool IsOpen() => isOpen;
    public bool IsRecentlyClosed() => recentlyClosed;
}