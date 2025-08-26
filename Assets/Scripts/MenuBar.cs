using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pauseMenuUI;   // メニュー全体
    public GameObject charPanel;     // キャラパネル
    public GameObject itemPanel;     // アイテムパネル
    public GameObject documentPanel; // 資料パネル
    public GameObject optionPanel;   // オプションパネル
    public GameObject itemInfoPanel; // アイテムバー (上の説明部分)

    [Header("Menu Buttons")]
    public GameObject firstSelected;    // ESCで最初に選択するボタン (Items)
    public GameObject firstItemButton;  // ItemPanel内で最初に選択するボタン (Item1)

    public static bool isPaused = false;
    private GameObject currentPanel = null; // 現在開いている右側のパネル

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
                // もし右パネルが開いてるなら閉じてキャラパネルに戻す
                if (currentPanel != null)
                {
                    currentPanel.SetActive(false);
                    currentPanel = null;
                    charPanel.SetActive(true);

                    // ESCで戻ったらカーソルを「Items」に戻す
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

        // 選択解除
        EventSystem.current.SetSelectedGameObject(null);

        Debug.Log("ゲーム再開");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        charPanel.SetActive(true); // 最初はキャラパネル表示
        currentPanel = null;

        Time.timeScale = 0f;
        isPaused = true;

        //  メニューを開いた時は「Items」ボタンにカーソルを置く
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        Debug.Log("ポーズメニューを開いた");
    }

    // ▼ アイテムパネルを開く
    public void OpenItems()
    {
        charPanel.SetActive(false);

        if (currentPanel != null) currentPanel.SetActive(false);

        itemPanel.SetActive(true);
        if (itemInfoPanel != null) itemInfoPanel.SetActive(true); // アイテムバーも表示
        currentPanel = itemPanel;

        //  アイテムパネルを開いた時は「Item1」にカーソルを合わせる
        if (firstItemButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstItemButton);
        }

        Debug.Log("アイテム画面を開いた");
    }

    // ▼ 資料パネルを開く
    public void OpenDocuments()
    {
        SwitchPanel(documentPanel);
        Debug.Log("資料画面を開いた");
    }

    // ▼ オプションパネルを開く
    public void OpenOptions()
    {
        SwitchPanel(optionPanel);
        Debug.Log("オプション画面を開いた");
    }

    // ▼ 共通の切り替え処理
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
        Debug.Log("ゲーム終了");
        Application.Quit();
    }
}
