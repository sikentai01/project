using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;  // UI選択を操作するために必要

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;   // 左のポーズメニュー
    public GameObject itemPanel;     // 右のアイテム画面
    public GameObject firstSelected; // 最初に選択されるボタン（Itemsなど）

    public static bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        itemPanel.SetActive(false);   // 最初は非表示
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
        itemPanel.SetActive(false);   // アイテム画面も一緒に閉じる
        Time.timeScale = 1f;
        isPaused = false;

        // 選択解除（ゲーム再開時はカーソルを消す）
        EventSystem.current.SetSelectedGameObject(null);

        Debug.Log("ゲーム再開");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        //  最初に「アイテム」ボタンにカーソルを合わせる
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);

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
