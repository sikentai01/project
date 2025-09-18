using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance; // ← シングルトン追加

    [Header("UI Panels")]
    public GameObject pauseMenuUI;   // メニュー全体
    public GameObject charPanel;     // キャラパネル
    public GameObject itemPanel;     // アイテムパネル
    public GameObject documentPanel; // 資料パネル (GridとDetailの親)
    public GameObject optionPanel;   // オプションパネル
    public GameObject controlPanel;  // 操作説明パネル
    public GameObject itemInfoPanel; // アイテムバー (上の説明部分)

    [Header("Menu Buttons (左のメニュー用)")]
    public GameObject firstSelected;    // ESCで最初に選択するボタン (Items)
    public GameObject documentButton;   // 左メニューの Documents ボタン
    public GameObject optionButton;     // 左メニューの Options ボタン
    public GameObject controlButton;    // 左メニューの Controls ボタン

    [Header("Panel First Buttons (右のパネル用)")]
    public GameObject firstItemButton;     // ItemPanel内で最初に選択するボタン
    public GameObject firstDocumentButton; // DocumentPanel内で最初に選択するボタン
    public GameObject firstOptionButton;   // OptionPanel内で最初に選択するボタン

    [Header("Managers")]
    public DocumentManager documentManager; // Inspectorでセットする

    public static bool isPaused = false;
    private GameObject currentPanel = null;     // 現在開いている右側のパネル
    private GameObject lastMenuButton = null;   // 直前に開いたメニューのボタン

    void Awake()
    {
        // シングルトン設定
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        CloseAllPanels(); // 全部閉じる
        pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
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

        currentPanel = null;
        lastMenuButton = null;

        Time.timeScale = 1f;
        isPaused = false;

        EventSystem.current.SetSelectedGameObject(null);
        Debug.Log("ゲーム再開");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        charPanel.SetActive(true);
        currentPanel = null;

        Time.timeScale = 0f;
        isPaused = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        Debug.Log("ポーズメニューを開いた");
    }

    // ▼ アイテムパネル
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

            var slot = firstItemButton.GetComponent<ItemSlot>();
            if (slot != null) slot.OnSelectSlot();
        }

        Debug.Log("アイテム画面を開いた");
    }

    // ▼ 資料パネル
    public void OpenDocuments()
    {
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

        Debug.Log("資料画面を開いた");
    }

    // ▼ オプションパネル
    public void OpenOptions()
    {
        SwitchPanel(optionPanel);
        lastMenuButton = optionButton;

        if (firstOptionButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstOptionButton);
        }

        Debug.Log("オプション画面を開いた");
    }

    // ▼ 操作説明パネル
    public void OpenControls()
    {
        charPanel.SetActive(false);
        if (currentPanel != null) currentPanel.SetActive(false);

        controlPanel.SetActive(true);
        currentPanel = controlPanel;

        lastMenuButton = controlButton;

        Debug.Log("操作説明を開いた");
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
        Debug.Log("ゲーム終了");
        Application.Quit();
    }
}