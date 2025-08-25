using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // ← 追加（UI操作に必要）

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;   // Panelをアタッチ
    public GameObject firstButton;   // 最初に選択されるボタン (Itemsをアタッチ)
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);  // 最初は必ず非表示にする
        Time.timeScale = 1f;           // ゲームスピードもリセット
        isPaused = false;              // フラグもリセット
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

        // ▼ メニューを開いたときに Items ボタンにカーソルを合わせる
        EventSystem.current.SetSelectedGameObject(null);        // 一度リセット
        EventSystem.current.SetSelectedGameObject(firstButton); // Itemsを選択
    }

    // ▼ ダミーボタン処理 ▼
    public void OpenItems()
    {
        Debug.Log("アイテム画面を開く（ダミー）");
    }

    public void OpenDocuments()
    {
        Debug.Log("資料画面を開く（ダミー）");
    }

    public void OpenOptions()
    {
        Debug.Log("オプション画面を開く（ダミー）");
    }

    // ▼ 実際に処理するボタン ▼
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
