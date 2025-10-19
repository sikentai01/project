using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    private static BootLoader _instance;
    public static BootLoader Instance => _instance; // ←これを追加！

    private Dictionary<string, Scene> loadedScenes = new Dictionary<string, Scene>();

    [Header("起動時にロードしておくシーン")]
    public List<string> preloadScenes = new List<string> { "Title", "Scenes0", "Scenes01", "Scenes02", "GameOver" };

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(PreloadScenes());
    }

    private System.Collections.IEnumerator PreloadScenes()
    {
        foreach (var name in preloadScenes)
        {
            if (!SceneManager.GetSceneByName(name).isLoaded)
            {
                var op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                while (!op.isDone) yield return null;
                var scene = SceneManager.GetSceneByName(name);
                loadedScenes[name] = scene;
                SetSceneActive(name, name == "Title"); // 起動時はTitleだけON
            }
        }

        Debug.Log("[BootLoader] すべてのシーンを事前ロード完了。Title表示中。");
    }

    public void SetSceneActive(string sceneName, bool active)
    {
        if (!loadedScenes.ContainsKey(sceneName)) return;

        var scene = loadedScenes[sceneName];
        foreach (var root in scene.GetRootGameObjects())
            root.SetActive(active);

        Debug.Log($"[BootLoader] {sceneName} を {(active ? "有効化" : "無効化")} にしました。");
    }

    public void StartGame()
    {
        Debug.Log("[BootLoader] はじめから開始");

        // まずタイトルを非表示
        SetSceneActive("Title", false);

        // Scene0を有効化
        SetSceneActive("Scenes0", true);

        var scene = loadedScenes["Scenes0"];
        SceneManager.SetActiveScene(scene);

        // ★ ここではまだタイトルのUIが残ってる可能性があるので
        // コルーチンで1フレーム遅延させて解除する
        Instance.StartCoroutine(DelayedMenuUnlock());
    }

    private IEnumerator DelayedMenuUnlock()
    {
        yield return null; // 1フレーム待つ
        PauseMenu.blockMenu = false;
        Debug.Log("[BootLoader] メニューブロック解除（Scene切り替え後）");
    }

    public void ReturnToTitle()
    {
        Debug.Log("[BootLoader] タイトルに戻ります…");
        foreach (var kv in loadedScenes)
        {
            if (kv.Key != "Title")
                SetSceneActive(kv.Key, false);
        }

        SetSceneActive("Title", true);
        var titleScene = loadedScenes["Title"];
        SceneManager.SetActiveScene(titleScene);
    }
}