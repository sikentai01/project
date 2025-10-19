using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    private static BootLoader _instance;
    public static BootLoader Instance => _instance;

    private Dictionary<string, Scene> loadedScenes = new Dictionary<string, Scene>();

    [Header("�N�����Ƀ��[�h���Ă����V�[��")]
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
                SetSceneActive(name, name == "Title"); // �N������Title����ON
            }
        }

        Debug.Log("[BootLoader] ���ׂẴV�[�������O���[�h�����BTitle�\�����B");
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

        // �܂��^�C�g�����\��
        SetSceneActive("Title", false);

        // Scene0��L����
        SetSceneActive("Scenes0", true);

        var scene = loadedScenes["Scenes0"];
        SceneManager.SetActiveScene(scene);

        // �V�[���̐؂�ւ���A1�t���[���҂��ď�����
        StartCoroutine(InitializePlayerAfterSceneLoad());
    }

    private IEnumerator InitializePlayerAfterSceneLoad()
    {
        yield return null; // �V�[���؂�ւ��̊�����҂�

        // --- �v���C���[�T�� ---
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[BootLoader] �v���C���[��������܂���B");
            yield break;
        }

        // --- SpawnPoint�T�� ---
        var spawn = GameObject.Find("SpawnPoint");
        if (spawn != null)
        {
            player.transform.position = spawn.transform.position;

            var move = player.GetComponent<GridMovement>();
            if (move != null)
            {
                move.SetDirection(0); // ���������Z�b�g
            }

            Debug.Log("[BootLoader] �v���C���[�ʒu��SpawnPoint�ɏ���������");
        }
        else
        {
            Debug.LogWarning("[BootLoader] SpawnPoint��������܂���ł����B");
        }

        // --- ���j���[�u���b�N���� ---
        PauseMenu.blockMenu = false;
        Debug.Log("[BootLoader] ���j���[�u���b�N�����i��������j");
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