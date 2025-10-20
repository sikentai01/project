using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    private static BootLoader _instance;
    public static BootLoader Instance => _instance;

    public Dictionary<string, Scene> loadedScenes = new Dictionary<string, Scene>();

    [Header("起動時にロードしておくシーン")]
    public List<string> preloadScenes = new List<string> { "Title", "Scenes0", "Scenes01", "Scenes02", "GameOver" };

#if UNITY_EDITOR
    [Header("デバッグ起動設定（エディタ専用）")]
    [Tooltip("デバッグ時のみタイトルをスキップして直接このシーンを有効化します")]
    [SerializeField] private bool enableDebugStart = false;

    [Tooltip("開始したいシーン名（例：Scenes0、Scenes01など）")]
    [SerializeField] private string debugSceneName = "Scenes0";

    [Tooltip("BootLoaderのAdditiveロード完了後、待機する時間（秒）")]
    [SerializeField] private float waitForBoot = 1.0f;
#endif

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
#if UNITY_EDITOR
        // ===============================
        // デバッグスキップモード
        // ===============================
        if (enableDebugStart && !string.IsNullOrEmpty(debugSceneName))
        {
            Debug.Log($"[BootLoader] デバッグスキップ起動: '{debugSceneName}' を直接ロードします。");

            // TitleをOFF
            SetSceneActive("Title", false);

            // シーンロード
            AsyncOperation op = SceneManager.LoadSceneAsync(debugSceneName, LoadSceneMode.Additive);
            yield return new WaitUntil(() => op.isDone);

            var scene = SceneManager.GetSceneByName(debugSceneName);
            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(scene);
                Debug.Log($"[BootLoader] '{debugSceneName}' をアクティブシーンに設定しました。");
            }

            // ===== ここでセーブデータ自動ロードを追加 =====
            var data = SaveSystem.LoadGame(0);
            if (data != null)
            {
                Debug.Log("[BootLoader] セーブデータが存在したためロード開始。");
                GameBootstrap.loadedData = data;
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
            }
            else
            {
                Debug.LogWarning("[BootLoader] セーブデータが存在しません。空状態で開始します。");
            }

            // プレイヤー初期化
            StartCoroutine(InitializePlayerAfterSceneLoad());
            yield break;
        }
#endif

        // ===============================
        // 通常起動（タイトルから）
        // ===============================
        foreach (var name in preloadScenes)
        {
            if (!SceneManager.GetSceneByName(name).isLoaded)
            {
                var op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                while (!op.isDone) yield return null;

                var scene = SceneManager.GetSceneByName(name);
                loadedScenes[name] = scene;
                SetSceneActive(name, name == "Title");
            }
        }

        Debug.Log("[BootLoader] すべてのシーンをプリロード完了。Title表示中。");
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

        // Titleを非表示
        SetSceneActive("Title", false);
        SetSceneActive("Scenes0", true);

        var scene = loadedScenes["Scenes0"];
        SceneManager.SetActiveScene(scene);

        // 進行度リセット
        Debug.Log("[BootLoader] はじめからのため進行度をリセット中...");
        var triggers = Object.FindObjectsByType<ItemTrigger>(FindObjectsSortMode.None);
        foreach (var t in triggers)
            t.LoadProgress(0);

        GameFlags.Instance?.ClearAllFlags();

        StartCoroutine(InitializePlayerAfterSceneLoad());
    }

    private IEnumerator InitializePlayerAfterSceneLoad()
    {
        yield return null;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[BootLoader] プレイヤーが見つかりません。");
            yield break;
        }

#if UNITY_EDITOR
        if (enableDebugStart)
        {
            Debug.Log("[BootLoader] デバッグスキップ中のため、SpawnPoint無視でプレハブ座標使用。");
        }
        else
#endif
        {
            var spawn = GameObject.Find("SpawnPoint");
            if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
                Debug.Log("[BootLoader] プレイヤー位置をSpawnPointに初期化完了");
            }
        }

        var move = player.GetComponent<GridMovement>();
        if (move != null) move.SetDirection(0);

        PauseMenu.blockMenu = false;
        Debug.Log("[BootLoader] メニューブロック解除");
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