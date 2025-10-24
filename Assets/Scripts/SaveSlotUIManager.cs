using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SaveSlotUIManager : MonoBehaviour
{
    public static SaveSlotUIManager Instance;

    [Header("UI�Q��")]
    [SerializeField] private GameObject saveSlotCanvas;

    private bool isLoadMode = false;
    private bool isViewOnly = false; //  �ǉ�
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
    // �ʏ�Z�[�u���[�h
    // ======================
    public void OpenSavePanel()
    {
        isLoadMode = false;
        isViewOnly = false; //  �ʏ�
        OpenPanelInternal();
    }

    // ======================
    // ���[�h���[�h
    // ======================
    public void OpenLoadPanel()
    {
        isLoadMode = true;
        isViewOnly = false; // ���[�h�͒ʏ퓮��
        OpenPanelInternal();
    }


    // ======================
    // �r���[��p���[�h�i�Z�[�u�֎~�j
    // ======================
    public void OpenViewOnlyPanel()
    {
        isLoadMode = false;
        isViewOnly = true; //  �������̐S
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
            btn.SetMode(isLoadMode, isViewOnly);

        // --- �Ō�Ɏg�����X���b�g�ԍ���ǂݍ��� ---
        int lastSlot = PlayerPrefs.GetInt("LastUsedSlot", 1);
        lastSlot = Mathf.Clamp(lastSlot, 1, buttons.Length);

        // --- �J�[�\�����Ō�̃X���b�g�ɍ��킹�� ---
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttons[lastSlot - 1].gameObject);
        }

        if (PauseMenu.Instance != null)
            PauseMenu.blockMenu = true;

        Debug.Log($"[SaveSlotUIManager] {(isViewOnly ? "�{����p" : isLoadMode ? "���[�h" : "�Z�[�u")}���[�h�ŃX���b�g���J���܂����B");
    }

    // ======================
    // ���鏈��
    // ======================
    public void ClosePanel()
    {
        if (saveSlotCanvas == null) return;

        saveSlotCanvas.SetActive(false);
        isOpen = false;
        isViewOnly = false; //  �����烊�Z�b�g

        // --- ���j���[�ċ��� ---
        if (PauseMenu.Instance != null)
        {
            PauseMenu.blockMenu = false;
            PauseMenu.isPaused = false;
            Time.timeScale = 1f;
        }

        var player = FindFirstObjectByType<GridMovement>();
        if (player != null)
            player.enabled = true;

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
    public bool IsViewOnly() => isViewOnly; //  ���N���X������m�F��
}
