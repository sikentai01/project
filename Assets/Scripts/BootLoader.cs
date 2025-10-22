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
        if (enableDebugStart && !string.IsNullOrEmpty(debugSceneName))
        {
            Debug.Log($"[BootLoader] デバッグスキップ有効: {debugSceneName}をアクティブ化");

            foreach (var kv in loadedScenes) SetSceneActive(kv.Key, false);
            SetSceneActive("Bootstrap", true); // 残す（ある場合）

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
        foreach (var root in scene.GetRootGameObjects()) root.SetActive(active);

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
        if (move != null) move.SetDirection(0);
    }

    // ===== はじめから =====
    public void StartGame()
    {
        Debug.Log("[BootLoader] はじめから開始");

        SetSceneActive("Title", false);
        SetSceneActive("Scenes0", true);

        var scene = loadedScenes["Scenes0"];
        SceneManager.SetActiveScene(scene);

        Debug.Log("[BootLoader] 進行度リセット & 初期化中...");

        // 鍵付きドアだけリセット（鍵不要ドアは開いたまま）
        var doors = Object.FindObjectsByType<DoorController>(FindObjectsSortMode.None);
        foreach (var d in doors)
        {
            if (!string.IsNullOrEmpty(d.GetRequiredKeyID()))
            {
                d.LoadProgress(0);
                Debug.Log($"[BootLoader] {d.name}（鍵付き）をリセット");
            }
            else
            {
                d.LoadProgress(1);
                Debug.Log($"[BootLoader] {d.name}（鍵不要）を開いたまま維持");
            }
        }

        // アイテムトリガー初期化
        var triggers = Object.FindObjectsByType<ItemTrigger>(FindObjectsSortMode.None);
        foreach (var t in triggers) t.LoadProgress(0);

        GameFlags.Instance?.ClearAllFlags();
        StartCoroutine(InitializePlayerAfterSceneLoad(false));
    }

    // ===== タイトルへ =====
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

    // ===== ドアからのシーン切替 =====
    public void RequestSceneSwitch(string sceneName, string spawnPointName)
    {
        StartCoroutine(SwitchSceneRoutine(sceneName, spawnPointName));
    }

    private IEnumerator SwitchSceneRoutine(string sceneName, string spawnPointName)
    {
        Debug.Log($"[BootLoader] シーン切替要求: {sceneName} → Spawn='{spawnPointName}'");

        yield return null; // 1フレーム待機

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

        // 全シーンのON/OFF
        foreach (var kv in loadedScenes)
        {
            bool active = kv.Key == sceneName;
            SetSceneActive(kv.Key, active);
        }

        // アクティブ設定
        var targetScene = loadedScenes[sceneName];
        SceneManager.SetActiveScene(targetScene);

        // プレイヤー移動（SpawnPointが現れるまで待つ）
        yield return new WaitForEndOfFrame();
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 最大2秒だけ待つ（無限待ちは避ける）
            float timeout = Time.realtimeSinceStartup + 2f;
            GameObject spawn = null;
            while (Time.realtimeSinceStartup < timeout && (spawn = GameObject.Find(spawnPointName)) == null)
                yield return null;

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