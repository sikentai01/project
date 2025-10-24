using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SaveSlotUIManager : MonoBehaviour
{
    public static SaveSlotUIManager Instance;

    [Header("UI参照")]
    [SerializeField] private GameObject saveSlotCanvas;

    private bool isLoadMode = false;
    private bool isViewOnly = false; //  追加
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
    // 通常セーブモード
    // ======================
    public void OpenSavePanel()
    {
        isLoadMode = false;
        isViewOnly = false; //  通常
        OpenPanelInternal();
    }

    // ======================
    // ロードモード
    // ======================
    public void OpenLoadPanel()
    {
        isLoadMode = true;
        isViewOnly = false; // ロードは通常動作
        OpenPanelInternal();
    }


    // ======================
    // ビュー専用モード（セーブ禁止）
    // ======================
    public void OpenViewOnlyPanel()
    {
        isLoadMode = false;
        isViewOnly = true; //  ここが肝心
        OpenPanelInternal();
    }

    // ======================
    // 共通の開く処理
    // ======================
    private void OpenPanelInternal()
    {
        if (saveSlotCanvas == null) return;

        saveSlotCanvas.SetActive(true);
        isOpen = true;

        var buttons = saveSlotCanvas.GetComponentsInChildren<SaveSlotButton>(true);
        foreach (var btn in buttons)
            btn.SetMode(isLoadMode, isViewOnly);

        // --- 最後に使ったスロット番号を読み込み ---
        int lastSlot = PlayerPrefs.GetInt("LastUsedSlot", 1);
        lastSlot = Mathf.Clamp(lastSlot, 1, buttons.Length);

        // --- カーソルを最後のスロットに合わせる ---
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttons[lastSlot - 1].gameObject);
        }

        if (PauseMenu.Instance != null)
            PauseMenu.blockMenu = true;

        Debug.Log($"[SaveSlotUIManager] {(isViewOnly ? "閲覧専用" : isLoadMode ? "ロード" : "セーブ")}モードでスロットを開きました。");
    }

    // ======================
    // 閉じる処理
    // ======================
    public void ClosePanel()
    {
        if (saveSlotCanvas == null) return;

        saveSlotCanvas.SetActive(false);
        isOpen = false;
        isViewOnly = false; //  閉じたらリセット

        // --- メニュー再許可 ---
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

        Debug.Log("[SaveSlotUIManager] スロットUIを閉じました。");
    }

    private IEnumerator ResetRecentlyClosed()
    {
        yield return null;
        recentlyClosed = false;
    }

    public bool IsOpen() => isOpen;
    public bool IsRecentlyClosed() => recentlyClosed;
    public bool IsViewOnly() => isViewOnly; //  他クラスからも確認可
}
