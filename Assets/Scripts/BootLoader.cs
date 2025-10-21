using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    private static BootLoader _instance;
    public static BootLoader Instance => _instance;

    public Dictionary<string, Scene> loadedScenes = new Dictionary<string, Scene>();

    [Header("�N�����Ƀ��[�h���Ă����V�[��")]
    public List<string> preloadScenes = new List<string> { "Title", "Scenes0", "Scenes01", "Scenes02", "GameOver" };

#if UNITY_EDITOR
    [Header("�f�o�b�O�N���ݒ�i�G�f�B�^��p�j")]
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
        // --- �S�V�[��Additive���[�h ---
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
        // --- �f�o�b�O�X�L�b�v���L���ȏꍇ ---
        if (enableDebugStart && !string.IsNullOrEmpty(debugSceneName))
        {
            Debug.Log($"[BootLoader] �f�o�b�O�X�L�b�v�L��: {debugSceneName}���A�N�e�B�u��");

            foreach (var kv in loadedScenes)
                SetSceneActive(kv.Key, false);

            // Bootstrap�͎c��
            SetSceneActive("Bootstrap", true);

            if (loadedScenes.ContainsKey(debugSceneName))
            {
                SetSceneActive(debugSceneName, true);
                SceneManager.SetActiveScene(loadedScenes[debugSceneName]);
                Debug.Log($"[BootLoader] {debugSceneName}���A�N�e�B�u�V�[���ɐݒ�");
            }

            yield return new WaitForSeconds(waitForBoot);
            StartCoroutine(InitializePlayerAfterSceneLoad(true));
            yield break;
        }
#endif

        // --- �ʏ�N�� ---
        Debug.Log("[BootLoader] �ʏ�N��: Title�̂ݗL����");
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

        Debug.Log($"[BootLoader] {sceneName} �� {(active ? "�L����" : "������")} �ɂ��܂����B");
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
        if (move != null)
            move.SetDirection(0);
    }

    // ============================
    // �͂��߂���J�n
    // ============================
    public void StartGame()
    {
        Debug.Log("[BootLoader] �͂��߂���J�n");

        // �^�C�g����\��
        SetSceneActive("Title", false);

        // �ŏ��̃V�[���iScenes0�Ȃǁj��L����
        SetSceneActive("Scenes0", true);

        // �A�N�e�B�u�V�[���ݒ�
        var scene = loadedScenes["Scenes0"];
        SceneManager.SetActiveScene(scene);

        Debug.Log("[BootLoader] �i�s�x���Z�b�g & ��������...");

        // �A�C�e���g���K�[������
        var triggers = Object.FindObjectsByType<ItemTrigger>(FindObjectsSortMode.None);
        foreach (var t in triggers)
        {
            t.LoadProgress(0);
        }

        // �t���O���Z�b�g
        GameFlags.Instance?.ClearAllFlags();

        // �v���C���[������
        StartCoroutine(InitializePlayerAfterSceneLoad(false));
    }

    // ============================
    // �^�C�g���ɖ߂�
    // ============================
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

    // ============================
    //  �h�A����̃V�[���؂�ւ��i�V�K�ǉ��j
    // ============================
    public void RequestSceneSwitch(string sceneName, string spawnPointName)
    {
        StartCoroutine(SwitchSceneRoutine(sceneName, spawnPointName));
    }

    private IEnumerator SwitchSceneRoutine(string sceneName, string spawnPointName)
    {
        Debug.Log($"[BootLoader] �V�[���ؑ֗v��: {sceneName} �� Spawn='{spawnPointName}'");

        yield return null; // 1�t���[���ҋ@���Ă���ؑ�

        if (!loadedScenes.ContainsKey(sceneName))
        {
            var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!async.isDone) yield return null;

            var newScene = SceneManager.GetSceneByName(sceneName);
            if (newScene.IsValid())
            {
                loadedScenes[sceneName] = newScene;
                Debug.Log($"[BootLoader] {sceneName} �� Additive���[�h����");
            }
        }

        // --- �S�V�[����ON/OFF���� ---
        foreach (var kv in loadedScenes)
        {
            bool active = kv.Key == sceneName;
            SetSceneActive(kv.Key, active);
        }

        // --- �V�[���A�N�e�B�u�ݒ� ---
        var targetScene = loadedScenes[sceneName];
        SceneManager.SetActiveScene(targetScene);

        // --- �v���C���[�ړ� ---
        yield return new WaitForEndOfFrame();
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var spawn = GameObject.Find(spawnPointName);
            if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
                Physics2D.SyncTransforms();
                Debug.Log($"[BootLoader] �v���C���[�� {spawnPointName} �Ɉړ� ({sceneName})");
            }
            else
            {
                Debug.LogWarning($"[BootLoader] SpawnPoint '{spawnPointName}' ��������܂���");
            }
        }
    }
}