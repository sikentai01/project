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
    [Header(" デバッグ起動設定（エディタ専用）")]
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
        if (enableDebugStart && !string.IsNullOrEmpty(debugSceneName))
        {
            Debug.Log($"[BootLoader] デバッグスキップ: '{debugSceneName}' を Additive モードでロードします。");

            // Titleシーンを無効化（Additiveで残ってるなら）
            SetSceneActive("Title", false);

            // 対象シーンを Additive でロード
            AsyncOperation op = SceneManager.LoadSceneAsync(debugSceneName, LoadSceneMode.Additive);
            op.completed += _ =>
            {
                var scene = SceneManager.GetSceneByName(debugSceneName);
                if (scene.IsValid())
                {
                    SceneManager.SetActiveScene(scene);
                    Debug.Log($"[BootLoader] '{debugSceneName}' をアクティブシーンに設定");
                }

                // プレイヤー初期化（SpawnPoint無視でプレハブ位置使用）
                StartCoroutine(InitializePlayerAfterSceneLoad());
            };

            yield break; // 通常の Additive 全ロード処理をスキップ
        }
#endif
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

#if UNITY_EDITOR
        // デバッグスキップ有効ならタイトルを無効化して指定シーンを有効化
        if (enableDebugStart && !string.IsNullOrEmpty(debugSceneName))
        {
            yield return new WaitForSeconds(waitForBoot);
            DebugSkipToScene(debugSceneName);
        }
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// デバッグスキップで指定シーンを有効化する
    /// </summary>
    private void DebugSkipToScene(string sceneName)
    {
        if (!loadedScenes.ContainsKey(sceneName))
        {
            Debug.LogWarning($"[BootLoader] デバッグシーン '{sceneName}' がプリロードされていません。preloadScenes に追加してください。");
            return;
        }

        Debug.Log($"[BootLoader]  デバッグモード有効: Titleをスキップして '{sceneName}' をアクティブ化します。");

        // TitleをOFF
        SetSceneActive("Title", false);

        // 対象シーンをON
        SetSceneActive(sceneName, true);

        // アクティブシーン設定
        var scene = loadedScenes[sceneName];
        SceneManager.SetActiveScene(scene);

        // プレイヤー初期化
        StartCoroutine(InitializePlayerAfterSceneLoad());
    }
#endif

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

        // --- ここを追加 ---
        Debug.Log("[BootLoader] はじめからのため進行度をリセット中…");
        var triggers = Object.FindObjectsByType<ItemTrigger>(FindObjectsSortMode.None);
        foreach (var t in triggers)
        {
            t.LoadProgress(0);  // 進行度を強制的に0に戻す
        }

        GameFlags.Instance?.ClearAllFlags(); // ←必要ならフラグもリセット

        // プレイヤー初期化
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
        // デバッグスキップ時はSpawnPointを無視し、プレハブ座標を使用
        if (enableDebugStart)
        {
            Debug.Log($"[BootLoader] デバッグスキップ中のため、SpawnPoint無視。プレハブ初期座標 {player.transform.position} を使用します。");
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
            else
            {
                Debug.LogWarning("[BootLoader] SpawnPointが見つかりませんでした。プレハブ初期位置を使用します。");
            }
        }

        var move = player.GetComponent<GridMovement>();
        if (move != null)
            move.SetDirection(0);

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