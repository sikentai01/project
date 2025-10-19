using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static bool isGameOver = false; // ←★追加：ゲームオーバーフラグ

    [SerializeField] private AudioClip confirmSeClip;

    void OnEnable()
    {
        isGameOver = true;
        Time.timeScale = 0f; // ←★ゲーム内の動きを止める
    }

    void OnDisable()
    {
        isGameOver = false;
        Time.timeScale = 1f; // ←★再開時戻す
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("[GameOverController] タイトルに戻る");

            if (SoundManager.Instance != null && confirmSeClip != null)
                SoundManager.Instance.PlaySE(confirmSeClip);

            ReturnToTitle();
        }
    }

    private void ReturnToTitle()
    {
        // ゲームシーンを全て非アクティブ化
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name.StartsWith("Scene"))
            {
                foreach (var root in s.GetRootGameObjects())
                    root.SetActive(false);
            }
        }

        // GameOver UIを非表示
        Scene current = SceneManager.GetSceneByName("GameOver");
        if (current.isLoaded)
        {
            foreach (var root in current.GetRootGameObjects())
                root.SetActive(false);
        }

        // Titleを再有効化
        Scene titleScene = SceneManager.GetSceneByName("Title");
        if (titleScene.isLoaded)
        {
            foreach (var root in titleScene.GetRootGameObjects())
                root.SetActive(true);
        }
        else
        {
            SceneManager.LoadSceneAsync("Title", LoadSceneMode.Additive);
        }

        Debug.Log("[GameOverController] タイトルへの復帰完了");
    }
}