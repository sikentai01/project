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
    [Tooltip("�f�o�b�O���̂݃^�C�g�����X�L�b�v���Ē��ڂ��̃V�[����L�������܂�")]
    [SerializeField] private bool enableDebugStart = false;

    [Tooltip("�J�n�������V�[�����i��FScenes0�AScenes01�Ȃǁj")]
    [SerializeField] private string debugSceneName = "Scenes0";

    [Tooltip("BootLoader��Additive���[�h������A�ҋ@���鎞�ԁi�b�j")]
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
        // �f�o�b�O�X�L�b�v���[�h
        // ===============================
        if (enableDebugStart && !string.IsNullOrEmpty(debugSceneName))
        {
            Debug.Log($"[BootLoader] �f�o�b�O�X�L�b�v�N��: '{debugSceneName}' �𒼐ڃ��[�h���܂��B");

            // Title��OFF
            SetSceneActive("Title", false);

            // �V�[�����[�h
            AsyncOperation op = SceneManager.LoadSceneAsync(debugSceneName, LoadSceneMode.Additive);
            yield return new WaitUntil(() => op.isDone);

            var scene = SceneManager.GetSceneByName(debugSceneName);
            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(scene);
                Debug.Log($"[BootLoader] '{debugSceneName}' ���A�N�e�B�u�V�[���ɐݒ肵�܂����B");
            }

            // ===== �����ŃZ�[�u�f�[�^�������[�h��ǉ� =====
            var data = SaveSystem.LoadGame(0);
            if (data != null)
            {
                Debug.Log("[BootLoader] �Z�[�u�f�[�^�����݂������߃��[�h�J�n�B");
                GameBootstrap.loadedData = data;
                new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
            }
            else
            {
                Debug.LogWarning("[BootLoader] �Z�[�u�f�[�^�����݂��܂���B���ԂŊJ�n���܂��B");
            }

            // �v���C���[������
            StartCoroutine(InitializePlayerAfterSceneLoad());
            yield break;
        }
#endif

        // ===============================
        // �ʏ�N���i�^�C�g������j
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

        Debug.Log("[BootLoader] ���ׂẴV�[�����v�����[�h�����BTitle�\�����B");
    }

    public void SetSceneActive(string sceneName, bool active)
    {
        if (!loadedScenes.ContainsKey(sceneName)) return;

        var scene = loadedScenes[sceneName];
        foreach (var root in scene.GetRootGameObjects())
            root.SetActive(active);

        Debug.Log($"[BootLoader] {sceneName} �� {(active ? "�L����" : "������")} �ɂ��܂����B");
    }

    public void StartGame()
    {
        Debug.Log("[BootLoader] �͂��߂���J�n");

        // Title���\��
        SetSceneActive("Title", false);
        SetSceneActive("Scenes0", true);

        var scene = loadedScenes["Scenes0"];
        SceneManager.SetActiveScene(scene);

        // �i�s�x���Z�b�g
        Debug.Log("[BootLoader] �͂��߂���̂��ߐi�s�x�����Z�b�g��...");
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
            Debug.LogWarning("[BootLoader] �v���C���[��������܂���B");
            yield break;
        }

#if UNITY_EDITOR
        if (enableDebugStart)
        {
            Debug.Log("[BootLoader] �f�o�b�O�X�L�b�v���̂��߁ASpawnPoint�����Ńv���n�u���W�g�p�B");
        }
        else
#endif
        {
            var spawn = GameObject.Find("SpawnPoint");
            if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
                Debug.Log("[BootLoader] �v���C���[�ʒu��SpawnPoint�ɏ���������");
            }
        }

        var move = player.GetComponent<GridMovement>();
        if (move != null) move.SetDirection(0);

        PauseMenu.blockMenu = false;
        Debug.Log("[BootLoader] ���j���[�u���b�N����");
    }

    public void ReturnToTitle()
    {
        Debug.Log("[BootLoader] �^�C�g���ɖ߂�܂��c");

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