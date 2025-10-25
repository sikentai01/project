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

    [Header("�N�����Ƀ��[�h���Ă����V�[��")]
    public List<string> preloadScenes = new List<string> { "Title", "Scenes0", "Scenes01", "Scenes02", "GameOver" };

    [Header("�t�F�[�h�ݒ�")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.6f;

#if UNITY_EDITOR
    [Header("�f�o�b�O�N���ݒ�i�G�f�B�^��p�j")]
    [SerializeField] private bool enableDebugStart = false;
    [SerializeField] private string debugSceneName = "Scenes01";
    [SerializeField] private float waitForBoot = 1.0f;
#endif

    private bool isFading = false;
    public static bool IsTransitioning { get; set; } = false;
    public static bool IsPlayerSpawning { get; private set; } = false; // ���� �ǉ�
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

        //  �t�F�[�h�����ݒ�
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
    //  �t�F�[�h�t���V�[���ؑցi�����Łj
    // ===================================================
    public void RequestSceneSwitch(string sceneName, string spawnPointName)
    {
        StartCoroutine(SwitchSceneRoutineWithFade(sceneName, spawnPointName));
    }

    private IEnumerator SwitchSceneRoutineWithFade(string sceneName, string spawnPointName)
    {
        Debug.Log($"[BootLoader] �t�F�[�h�t���V�[���ؑ֊J�n: {sceneName}");

        PauseMenu.blockMenu = true; // ���j���[�֎~

        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player?.GetComponent<GridMovement>();
        if (move != null) move.enabled = false; // �L�����ړ��֎~

        yield return FadeOut();

        yield return StartCoroutine(SwitchSceneRoutine(sceneName, spawnPointName));

        yield return FadeIn();

        PauseMenu.blockMenu = false; // ���j���[����

        player = GameObject.FindGameObjectWithTag("Player");
        move = player?.GetComponent<GridMovement>();
        if (move != null) move.enabled = true; // �L�����ړ��ĊJ

        Debug.Log($"[BootLoader] �t�F�[�h�t���V�[���ؑ֊���: {sceneName}");
    }

    // ===================================================
    //  �Ó]����
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
    //  ���]����
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
    //  �V�[���v�����[�h
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
                Debug.Log($"[BootLoader] {name} ��Additive���[�h����");
            }
        }

#if UNITY_EDITOR
        if (enableDebugStart && !string.IsNullOrEmpty(debugSceneName))
        {
            Debug.Log($"[BootLoader] �f�o�b�O�X�L�b�v�L��: {debugSceneName}���A�N�e�B�u��");

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

        Debug.Log("[BootLoader] �ʏ�N��: Title�̂ݗL����");
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
            Debug.LogWarning("[BootLoader] �v���C���[��������܂���B");
            yield break;
        }

#if UNITY_EDITOR
        if (keepPosition)
        {
            Debug.Log($"[BootLoader] �f�o�b�O�X�L�b�v���F���݂̔z�u�ʒu���ێ� ({player.transform.position})");
            yield break;
        }
#endif

        var spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
        {
            player.transform.position = spawn.transform.position;
            Debug.Log("[BootLoader] �v���C���[��SpawnPoint�ɏ�����");
        }

        var move = player.GetComponent<GridMovement>();
        if (move != null) move.SetDirection(0);
    }

    // ===================================================
    //  �͂��߂���
    // ===================================================
    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        Debug.Log("[BootLoader] �͂��߂���J�n");
        PauseMenu.blockMenu = true;
        IsPlayerSpawning = true; // ���� �v���C���[�����ړ����t���OON

        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player?.GetComponent<GridMovement>();
        if (move != null) move.enabled = false; // �L�����ړ��֎~

        yield return FadeOut();

        SetSceneActive("Title", false);
        SetSceneActive("Scenes0", true);

        var scene = loadedScenes["Scenes0"];
        SceneManager.SetActiveScene(scene);

        // �i�s�x������
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
        if (move != null) move.enabled = true; // �L�����ړ��ĊJ

        IsPlayerSpawning = false; // ���� ������������t���O����
        Debug.Log("[BootLoader] �Q�[���J�n����");
        yield return new WaitForSeconds(0.1f); // ������Ƒ҂��Ă���J���i���S��j

        var pauseMenu = PauseMenu.Instance;
        if (pauseMenu != null)
        {
            pauseMenu.Pause();         // �ʏ�̃��j���[���J��
            pauseMenu.OpenControls();  // �u��������v�^�u�ɐ؂�ւ�
            Debug.Log("[BootLoader] ��������^�u�������ŊJ���܂��� (Esc�ŕ���)");
        }
    }

    // ===================================================
    //  �^�C�g���֖߂�
    // ===================================================
    public void ReturnToTitle()
    {
        Debug.Log("[BootLoader] �^�C�g���ɖ߂�܂�...");

        foreach (var kv in loadedScenes)
        {
            bool active = kv.Key == "Title";
            SetSceneActive(kv.Key, active);
        }
        var titleScene = loadedScenes["Title"];
        SceneManager.SetActiveScene(titleScene);
    }



    // ===================================================
    //  �ʏ�̃V�[���ؑ�
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
    //  �����V�[���؂�ւ��iGameOver�E�^�C�g���p�j
    // ===================================================
    public void SwitchSceneInstant(string targetSceneName)
    {
        StartCoroutine(SwitchSceneInstantRoutine(targetSceneName));
    }

    private IEnumerator SwitchSceneInstantRoutine(string targetSceneName)
    {
        Debug.Log($"[BootLoader] �����V�[���؂�ւ��J�n: {targetSceneName}");

        // �v���C���[��~�{���Z�b�g
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

        // �V�[���؂�ւ�
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

        // �I����Ɉړ��ĊJ
        if (player != null)
        {
            var move = player.GetComponent<GridMovement>();
            if (move != null)
            {
                move.enabled = true;
                move.ForceStopMovement();
            }
        }

        Debug.Log($"[BootLoader] �����V�[���؂�ւ�����: {targetSceneName}");
    }
}