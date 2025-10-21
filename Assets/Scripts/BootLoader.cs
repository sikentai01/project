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
    [SerializeField] private bool enableDebugStart = false;
    [SerializeField] private string debugSceneName = "Scenes01";
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
        // --- 全シーンAdditiveロード ---
        foreach (var name in preloadScenes)
        {
            if (!SceneManager.GetSceneByName(name).isLoaded)
            {
                var op = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
                while (!op.isDone) yield return null;

                var scene = SceneManager.GetSceneByName(name);
                loadedScenes[name] = scene;
                Debug.Log($"[BootLoader] {name} をAdditiveロード完了");
            }
        }

#if UNITY_EDITOR
        // --- デバッグスキップが有効な場合 ---
        if (enableDebugStart && !string.IsNullOrEmpty(debugSceneName))
        {
            Debug.Log($"[BootLoader] デバッグスキップ有効: {debugSceneName}をアクティブ化");

            foreach (var kv in loadedScenes)
                SetSceneActive(kv.Key, false);

            // Bootstrapは残す
            SetSceneActive("Bootstrap", true);

            if (loadedScenes.ContainsKey(debugSceneName))
            {
                SetSceneActive(debugSceneName, true);
                SceneManager.SetActiveScene(loadedScenes[debugSceneName]);
                Debug.Log($"[BootLoader] {debugSceneName}をアクティブシーンに設定");
            }

            yield return new WaitForSeconds(waitForBoot);
            StartCoroutine(InitializePlayerAfterSceneLoad(true));
            yield break;
        }
#endif

        // --- 通常起動 ---
        Debug.Log("[BootLoader] 通常起動: Titleのみ有効化");
        foreach (var kv in loadedScenes)
        {
            bool active = kv.Key == "Title";
            SetSceneActive(kv.Key, active);
        }

        var titleScene = loadedScenes["Title"];
        SceneManager.SetActiveScene(titleScene);
    }

    public void SetSceneActive(string sceneName, bool active)
    {
        if (!loadedScenes.ContainsKey(sceneName)) return;

        var scene = loadedScenes[sceneName];
        foreach (var root in scene.GetRootGameObjects())
            root.SetActive(active);

        Debug.Log($"[BootLoader] {sceneName} を {(active ? "有効化" : "無効化")} にしました。");
    }

    private IEnumerator InitializePlayerAfterSceneLoad(bool keepPosition)
    {
        yield return null;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[BootLoader] プレイヤーが見つかりません。");
            yield break;
        }

#if UNITY_EDITOR
        if (keepPosition)
        {
            Debug.Log($"[BootLoader] デバッグスキップ中：現在の配置位置を維持 ({player.transform.position})");
            yield break;
        }
#endif

        var spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
        {
            player.transform.position = spawn.transform.position;
            Debug.Log("[BootLoader] プレイヤーをSpawnPointに初期化");
        }

        var move = player.GetComponent<GridMovement>();
        if (move != null)
            move.SetDirection(0);
    }

    // ============================
    // はじめから開始
    // ============================
    public void StartGame()
    {
        Debug.Log("[BootLoader] はじめから開始");

        // タイトル非表示
        SetSceneActive("Title", false);

        // 最初のシーン（Scenes0など）を有効化
        SetSceneActive("Scenes0", true);

        // アクティブシーン設定
        var scene = loadedScenes["Scenes0"];
        SceneManager.SetActiveScene(scene);

        Debug.Log("[BootLoader] 進行度リセット & 初期化中...");

        // アイテムトリガー初期化
        var triggers = Object.FindObjectsByType<ItemTrigger>(FindObjectsSortMode.None);
        foreach (var t in triggers)
        {
            t.LoadProgress(0);
        }

        // フラグリセット
        GameFlags.Instance?.ClearAllFlags();

        // プレイヤー初期化
        StartCoroutine(InitializePlayerAfterSceneLoad(false));
    }

    // ============================
    // タイトルに戻る
    // ============================
    public void ReturnToTitle()
    {
        Debug.Log("[BootLoader] タイトルに戻ります...");

        foreach (var kv in loadedScenes)
        {
            bool active = kv.Key == "Title";
            SetSceneActive(kv.Key, active);
        }

        var titleScene = loadedScenes["Title"];
        SceneManager.SetActiveScene(titleScene);
    }

    // ============================
    //  ドアからのシーン切り替え（新規追加）
    // ============================
    public void RequestSceneSwitch(string sceneName, string spawnPointName)
    {
        StartCoroutine(SwitchSceneRoutine(sceneName, spawnPointName));
    }

    private IEnumerator SwitchSceneRoutine(string sceneName, string spawnPointName)
    {
        Debug.Log($"[BootLoader] シーン切替要求: {sceneName} → Spawn='{spawnPointName}'");

        yield return null; // 1フレーム待機してから切替

        if (!loadedScenes.ContainsKey(sceneName))
        {
            var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!async.isDone) yield return null;

            var newScene = SceneManager.GetSceneByName(sceneName);
            if (newScene.IsValid())
            {
                loadedScenes[sceneName] = newScene;
                Debug.Log($"[BootLoader] {sceneName} を Additiveロード完了");
            }
        }

        // --- 全シーンのON/OFF制御 ---
        foreach (var kv in loadedScenes)
        {
            bool active = kv.Key == sceneName;
            SetSceneActive(kv.Key, active);
        }

        // --- シーンアクティブ設定 ---
        var targetScene = loadedScenes[sceneName];
        SceneManager.SetActiveScene(targetScene);

        // --- プレイヤー移動 ---
        yield return new WaitForEndOfFrame();
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var spawn = GameObject.Find(spawnPointName);
            if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
                Physics2D.SyncTransforms();
                Debug.Log($"[BootLoader] プレイヤーを {spawnPointName} に移動 ({sceneName})");
            }
            else
            {
                Debug.LogWarning($"[BootLoader] SpawnPoint '{spawnPointName}' が見つかりません");
            }
        }
    }
}