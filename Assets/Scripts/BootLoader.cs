using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    private static BootLoader _instance;
    public static BootLoader Instance => _instance;

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

    private IEnumerator PreloadScenes()
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

        // シーンの切り替え後、1フレーム待って初期化
        StartCoroutine(InitializePlayerAfterSceneLoad());
    }

    private IEnumerator InitializePlayerAfterSceneLoad()
    {
        yield return null; // シーン切り替えの完了を待つ

        // --- プレイヤー探す ---
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[BootLoader] プレイヤーが見つかりません。");
            yield break;
        }

        // --- SpawnPoint探す ---
        var spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
        {
            player.transform.position = spawn.transform.position;

            var move = player.GetComponent<GridMovement>();
            if (move != null)
            {
                move.SetDirection(0); // 下向きリセット
            }

            Debug.Log("[BootLoader] プレイヤー位置をSpawnPointに初期化完了");
        }
        else
        {
            Debug.LogWarning("[BootLoader] SpawnPointが見つかりませんでした。");
        }

        // --- メニューブロック解除 ---
        PauseMenu.blockMenu = false;
        Debug.Log("[BootLoader] メニューブロック解除（初期化後）");
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