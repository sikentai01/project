using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SaveSlotUIManager : MonoBehaviour
{
    public static SaveSlotUIManager Instance;

    [Header("UI参照")]
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
    // セーブモードで開く
    // ======================
    public void OpenSavePanel()
    {
        isLoadMode = false;
        OpenPanelInternal();
    }

    // ======================
    // ロードモードで開く
    // ======================
    public void OpenLoadPanel()
    {
        isLoadMode = true;
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
            btn.SetMode(isLoadMode);

        // --- 最初のスロットを選択 ---
        if (buttons.Length > 0 && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        }

        if (PauseMenu.Instance != null)
            PauseMenu.blockMenu = true;

        Debug.Log($"[SaveSlotUIManager] {(isLoadMode ? "ロード" : "セーブ")}スロットを開きました。");
    }

    // ======================
    // 閉じる処理
    // ======================
    public void ClosePanel()
    {
        if (saveSlotCanvas == null) return;

        saveSlotCanvas.SetActive(false);
        isOpen = false;

        // --- メニューを再有効化 ---
        if (PauseMenu.Instance != null)
        {
            PauseMenu.blockMenu = false;
            PauseMenu.isPaused = false;
            Time.timeScale = 1f;
        }

        // --- ゲーム中はプレイヤー再有効化 ---
        var player = FindFirstObjectByType<GridMovement>();
        if (player != null)
            player.enabled = true;

        // --- タイトル画面でロード閉じたとき ---
        if (isLoadMode && EventSystem.current != null)
        {
            var continueButton = GameObject.Find("ContinueButton"); // タイトルシーン上の名前に合わせる
            if (continueButton != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(continueButton);
                Debug.Log("[SaveSlotUIManager] カーソルを続きからに戻しました。");
            }
        }

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
}