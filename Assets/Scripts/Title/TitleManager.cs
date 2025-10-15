using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleMenuManager : MonoBehaviour
{
    [Header("初期選択ボタン設定")]
    [SerializeField] private GameObject firstSelectedButton; // Startボタンをアサイン

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    // --- 各ボタン用処理 ---

    /// <summary>
    /// 「初めから」ボタンを押したときに Scene0 に移動
    /// </summary>
    public void OnStartGame()
    {
        Debug.Log("ゲーム開始");

        // Additiveロード（Bootstrapを保持）
        RoomLoader.LoadRoom("Scenes0",null);

        // Titleを後から破棄
        StartCoroutine(UnloadTitleScene());
    }

    private IEnumerator UnloadTitleScene()
    {
        yield return new WaitForSeconds(0.5f);
        Scene current = SceneManager.GetSceneByName("Title");
        if (current.IsValid() && current.isLoaded)
        {
            SceneManager.UnloadSceneAsync(current);
            Debug.Log("Titleシーンを破棄しました");
        }
    }

    public void OnLoadGame()
    {
        Debug.Log("ロード機能は後で追加予定");
    }

    public void OnExitGame()
    {
        Debug.Log("ゲーム終了");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
