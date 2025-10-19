using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static bool isGameOver = false; // ←★ 追加：全体から参照できるフラグ

    [SerializeField] private AudioClip confirmSeClip;

    void OnEnable()
    {
        isGameOver = true; // 有効化されたらゲームオーバー状態ON
        Debug.Log("[GameOverController] ゲームオーバー状態になりました");
    }

    void OnDisable()
    {
        isGameOver = false; // 無効化されたら解除
        Debug.Log("[GameOverController] ゲームオーバー状態解除");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("[GameOverController] タイトルに戻る");

            // 効果音
            if (SoundManager.Instance != null && confirmSeClip != null)
                SoundManager.Instance.PlaySE(confirmSeClip);

            ReturnToTitle();
        }
    }

    private void ReturnToTitle()
    {
        // ゲームシーンを全部非アクティブ化
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name.StartsWith("Scene"))
            {
                foreach (var root in s.GetRootGameObjects())
                    root.SetActive(false);
            }
        }

        // GameOver自体非表示
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

        Debug.Log("[GameOverController] タイトル復帰完了");
    }
}