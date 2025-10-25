using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootLoader : MonoBehaviour
{
    private static BootLoader _instance;
    public static BootLoader Instance => _instance;

    public Dictionary<string, Scene> loadedScenes = new Dictionary<string, Scene>();

    [Header("起動時にロードしておくシーン")]
    public List<string> preloadScenes = new List<string> { "Title", "Scenes0", "Scenes01", "Scenes02", "GameOver" };

    [Header("フェード設定")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.6f;

#if UNITY_EDITOR
    [Header("デバッグ起動設定（エディタ専用）")]
    [SerializeField] private bool enableDebugStart = false;
    [SerializeField] private string debugSceneName = "Scenes01";
    [SerializeField] private float waitForBoot = 1.0f;
#endif

    private bool isFading = false;
    public static bool IsTransitioning { get; set; } = false;
    public static bool IsPlayerSpawning { get; private set; } = false; // ←★ 追加
    public static bool HasBooted { get; private set; } = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        //  フェード初期設定
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        StartCoroutine(PreloadScenes());
    }

    // ===================================================
    //  フェード付きシーン切替（統合版）
    // ===================================================
    public void RequestSceneSwitch(string sceneName, string spawnPointName)
    {
        StartCoroutine(SwitchSceneRoutineWithFade(sceneName, spawnPointName));
    }

    private IEnumerator SwitchSceneRoutineWithFade(string sceneName, string spawnPointName)
    {
        Debug.Log($"[BootLoader] フェード付きシーン切替開始: {sceneName}");

        PauseMenu.blockMenu = true; // メニュー禁止

        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player?.GetComponent<GridMovement>();
        if (move != null) move.enabled = false; // キャラ移動禁止

        yield return FadeOut();

        yield return StartCoroutine(SwitchSceneRoutine(sceneName, spawnPointName));

        yield return FadeIn();

        PauseMenu.blockMenu = false; // メニュー許可

        player = GameObject.FindGameObjectWithTag("Player");
        move = player?.GetComponent<GridMovement>();
        if (move != null) move.enabled = true; // キャラ移動再開

        Debug.Log($"[BootLoader] フェード付きシーン切替完了: {sceneName}");
    }

    // ===================================================
    //  暗転処理
    // ===================================================
    private IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);
        isFading = true;
        float t = 0f;
        Color c = fadeImage.color;
        c.a = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.SmoothStep(0f, 1f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;
        isFading = false;
    }

    // ===================================================
    //  明転処理
    // ===================================================
    private IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true);
        isFading = true;
        float t = 0f;
        Color c = fadeImage.color;
        c.a = 1f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.SmoothStep(1f, 0f, t / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        fadeImage.color = c;
        fadeImage.gameObject.SetActive(false);
        isFading = false;
    }

    // ===================================================
    //  シーンプリロード
    // ===================================================
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
            SetSceneActive("Bootstrap", true);

            if (loadedScenes.ContainsKey(debugSceneName))
            {
                SetSceneActive(debugSceneName, true);
                SceneManager.SetActiveScene(loadedScenes[debugSceneName]);
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
        HasBooted = true;
    }

    public void SetSceneActive(string sceneName, bool active)
    {
        if (!loadedScenes.ContainsKey(sceneName)) return;
        var scene = loadedScenes[sceneName];
        foreach (var root in scene.GetRootGameObjects()) root.SetActive(active);
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

    // ===================================================
    //  はじめから
    // ===================================================
    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        Debug.Log("[BootLoader] はじめから開始");
        PauseMenu.blockMenu = true;
        IsPlayerSpawning = true; // ←★ プレイヤー初期移動中フラグON

        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player?.GetComponent<GridMovement>();
        if (move != null) move.enabled = false; // キャラ移動禁止

        yield return FadeOut();

        SetSceneActive("Title", false);
        SetSceneActive("Scenes0", true);

        var scene = loadedScenes["Scenes0"];
        SceneManager.SetActiveScene(scene);

        // 進行度初期化
        var doors = Object.FindObjectsByType<DoorController>(FindObjectsSortMode.None);
        foreach (var d in doors)
        {
            if (!string.IsNullOrEmpty(d.GetRequiredKeyID())) d.LoadProgress(0);
            else d.LoadProgress(1);
        }

        var triggers = Object.FindObjectsByType<ItemTrigger>(FindObjectsSortMode.None);
        foreach (var t in triggers) t.LoadProgress(0);

        GameFlags.Instance?.ClearAllFlags();

        yield return InitializePlayerAfterSceneLoad(false);
        yield return FadeIn();

        PauseMenu.blockMenu = false;
        player = GameObject.FindGameObjectWithTag("Player");
        move = player?.GetComponent<GridMovement>();
        if (move != null) move.enabled = true; // キャラ移動再開

        IsPlayerSpawning = false; // ←★ 初期化完了後フラグ解除
        Debug.Log("[BootLoader] ゲーム開始完了");
        yield return new WaitForSeconds(0.1f); // ちょっと待ってから開く（安全策）

        var pauseMenu = PauseMenu.Instance;
        if (pauseMenu != null)
        {
            pauseMenu.Pause();         // 通常のメニューを開く
            pauseMenu.OpenControls();  // 「操作説明」タブに切り替え
            Debug.Log("[BootLoader] 操作説明タブを自動で開きました (Escで閉じる)");
        }
    }

    // ===================================================
    //  タイトルへ戻る
    // ===================================================
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



    // ===================================================
    //  通常のシーン切替
    // ===================================================
    private IEnumerator SwitchSceneRoutine(string sceneName, string spawnPointName)
    {
        yield return null;

        if (!loadedScenes.ContainsKey(sceneName))
        {
            var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!async.isDone) yield return null;
            var newScene = SceneManager.GetSceneByName(sceneName);
            loadedScenes[sceneName] = newScene;
        }

        foreach (var kv in loadedScenes)
        {
            bool active = kv.Key == sceneName;
            SetSceneActive(kv.Key, active);
        }

        var targetScene = loadedScenes[sceneName];
        SceneManager.SetActiveScene(targetScene);

        yield return new WaitForEndOfFrame();

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float timeout = Time.realtimeSinceStartup + 2f;
            GameObject spawn = null;
            while (Time.realtimeSinceStartup < timeout && (spawn = GameObject.Find(spawnPointName)) == null)
                yield return null;

            if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
                Physics2D.SyncTransforms();
            }
        }
    }
    // ===================================================
    //  即時シーン切り替え（GameOver・タイトル用）
    // ===================================================
    public void SwitchSceneInstant(string targetSceneName)
    {
        StartCoroutine(SwitchSceneInstantRoutine(targetSceneName));
    }

    private IEnumerator SwitchSceneInstantRoutine(string targetSceneName)
    {
        Debug.Log($"[BootLoader] 即時シーン切り替え開始: {targetSceneName}");

        // プレイヤー停止＋リセット
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var move = player.GetComponent<GridMovement>();
            if (move != null)
            {
                move.enabled = false;
                move.ForceStopMovement();
            }
        }

        yield return null;

        // シーン切り替え
        if (!loadedScenes.ContainsKey(targetSceneName))
        {
            var async = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            while (!async.isDone) yield return null;
            loadedScenes[targetSceneName] = SceneManager.GetSceneByName(targetSceneName);
        }

        foreach (var kv in loadedScenes)
        {
            bool active = kv.Key == targetSceneName;
            SetSceneActive(kv.Key, active);
        }

        SceneManager.SetActiveScene(loadedScenes[targetSceneName]);
        Physics2D.SyncTransforms();
        Input.ResetInputAxes();

        // 終了後に移動再開
        if (player != null)
        {
            var move = player.GetComponent<GridMovement>();
            if (move != null)
            {
                move.enabled = true;
                move.ForceStopMovement();
            }
        }

        Debug.Log($"[BootLoader] 即時シーン切り替え完了: {targetSceneName}");
    }
}