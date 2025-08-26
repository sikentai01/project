using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;   // メニュー全体
    public GameObject charPanel;     // キャラパネル
    public GameObject itemPanel;     // アイテムパネル
    public GameObject documentPanel; // 資料パネル
    public GameObject optionPanel;   // オプションパネル

    public GameObject firstSelected; // 最初に選択するボタン

    public static bool isPaused = false;
    private GameObject currentPanel = null; // 現在開いている右側のパネル

    void Start()
    {
        pauseMenuUI.SetActive(false);
        charPanel.SetActive(false);
        itemPanel.SetActive(false);
        documentPanel.SetActive(false);
        optionPanel.SetActive(false);

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
        currentPanel = null;

        Time.timeScale = 1f;
        isPaused = false;

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

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        Debug.Log("ポーズメニューを開いた");
    }

    // ▼ 各パネルを開く
    public void OpenItems()
    {
        SwitchPanel(itemPanel);
        Debug.Log("アイテム画面を開いた");
    }

    public void OpenDocuments()
    {
        SwitchPanel(documentPanel);
        Debug.Log("資料画面を開いた");
    }

    public void OpenOptions()
    {
        SwitchPanel(optionPanel);
        Debug.Log("オプション画面を開いた");
    }

    // ▼ 共通の切り替え処理
    void SwitchPanel(GameObject newPanel)
    {
        charPanel.SetActive(false);

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
