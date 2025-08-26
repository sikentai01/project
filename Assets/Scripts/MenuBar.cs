using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;  // 左のポーズメニュー
    public GameObject itemPanel;    // 右のアイテム画面 

    public static bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        itemPanel.SetActive(false);    // 最初はアイテム画面も非表示
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        itemPanel.SetActive(false);   // メニュー閉じる時に一緒に非表示
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("ゲーム再開");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Debug.Log("ポーズメニューを開いた");
    }

    // ▼ アイテム画面を開く
    public void OpenItems()
    {
        itemPanel.SetActive(true);
        Debug.Log("アイテム画面を開いた");
    }

    // ▼ アイテム画面を閉じる
    public void CloseItems()
    {
        itemPanel.SetActive(false);
    }

    public void OpenDocuments()
    {
        Debug.Log("資料画面を開く（ダミー）");
    }

    public void OpenOptions()
    {
        Debug.Log("オプション画面を開く（ダミー）");
    }

    public void LoadGame()
    {
        Debug.Log("ロード処理（ダミー or Scene切替を後で追加）");
        // SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("ゲーム終了");
        Application.Quit();
    }
}
