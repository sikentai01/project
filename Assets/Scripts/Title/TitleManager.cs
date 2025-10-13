using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleMenuManager : MonoBehaviour
{
    [Header("初期選択ボタン設定")]
    [SerializeField] private GameObject firstSelectedButton; // Startボタンをアサイン

    private void Start()
    {
        // タイトル開始時に START ボタンを選択状態にする
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    // --- 各ボタン用処理 ---

    /// <summary>
    /// 「初めから」ボタンを押したときに Scene0 に移動
    /// </summary>
    public void OnStartGame()
    {
        // シーン名かビルドインデックスどちらでもOK（ここでは名前で指定）
        SceneManager.LoadScene("Scenes0");
    }

    /// <summary>
    /// 「つづきから」ボタン用（未実装でもOK）
    /// </summary>
    public void OnLoadGame()
    {
        Debug.Log("ロード機能は後で追加予定");
    }

    /// <summary>
    /// 「終了」ボタンを押したとき
    /// </summary>
    public void OnExitGame()
    {
        Debug.Log("ゲーム終了");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディタ中なら再生停止
#endif
    }
}
