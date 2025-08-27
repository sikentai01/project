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

    [Header("Menu Buttons (左のメニュー用)")]
    public GameObject firstSelected;    // ESCで最初に選択するボタン (Items)
    public GameObject documentButton;   // 左メニューの Documents ボタン
    public GameObject optionButton;     // 左メニューの Options ボタン

    [Header("Panel First Buttons (右のパネル用)")]
    public GameObject firstItemButton;     // ItemPanel内で最初に選択するボタン (Item1)
    public GameObject firstDocumentButton; // DocumentPanel内で最初に選択するボタン
    public GameObject firstOptionButton;   // OptionPanel内で最初に選択するボタン

    public static bool isPaused = false;
    private GameObject currentPanel = null;     // 現在開いている右側のパネル
    private GameObject lastMenuButton = null;   // 直前に開いたメニューのボタン

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
                // 右パネルを開いている場合は閉じてキャラパネルに戻る
                if (currentPanel != null)
                {
                    currentPanel.SetActive(false);

                    // アイテムの場合はバーも閉じる
                    if (currentPanel == itemPanel && itemInfoPanel != null)
                        itemInfoPanel.SetActive(false);

                    currentPanel = null;
                    charPanel.SetActive(true);

                    //  ESCで戻ったら「直前に開いていたボタン」にカーソルを戻す
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
        pauseMenuUI.SetActive(false);
        charPanel.SetActive(false);
        itemPanel.SetActive(false);
        documentPanel.SetActive(false);
        optionPanel.SetActive(false);
        if (itemInfoPanel != null) itemInfoPanel.SetActive(false);

        currentPanel = null;
        lastMenuButton = null; //  Resumeしたらリセット（次回ESCではItems固定）

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

        //  メニューを開いたときは必ずItemsにカーソルを置く
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
        if (itemInfoPanel != null) itemInfoPanel.SetActive(true);
        currentPanel = itemPanel;

        lastMenuButton = firstSelected; //  ESC戻り先はItemsボタン

        // アイテムパネルを開いたらItem1にフォーカス
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
        lastMenuButton = documentButton; //  ESC戻り先はDocumentsボタン

        // 資料パネル内の最初の要素にフォーカス
        if (firstDocumentButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstDocumentButton);
        }

        Debug.Log("資料画面を開いた");
    }

    // ▼ オプションパネルを開く
    public void OpenOptions()
    {
        SwitchPanel(optionPanel);
        lastMenuButton = optionButton; //  ESC戻り先はOptionsボタン

        // オプションパネル内の最初の要素にフォーカス
        if (firstOptionButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstOptionButton);
        }

        Debug.Log("オプション画面を開いた");
    }

    // ▼ 共通の切り替え処理（アイテム以外用）
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
