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
    [Header(" �f�o�b�O�N���ݒ�i�G�f�B�^��p�j")]
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
        if (enableDebugStart && !string.IsNullOrEmpty(debugSceneName))
        {
            Debug.Log($"[BootLoader] �f�o�b�O�X�L�b�v: '{debugSceneName}' �� Additive ���[�h�Ń��[�h���܂��B");

            // Title�V�[���𖳌����iAdditive�Ŏc���Ă�Ȃ�j
            SetSceneActive("Title", false);

            // �ΏۃV�[���� Additive �Ń��[�h
            AsyncOperation op = SceneManager.LoadSceneAsync(debugSceneName, LoadSceneMode.Additive);
            op.completed += _ =>
            {
                var scene = SceneManager.GetSceneByName(debugSceneName);
                if (scene.IsValid())
                {
                    SceneManager.SetActiveScene(scene);
                    Debug.Log($"[BootLoader] '{debugSceneName}' ���A�N�e�B�u�V�[���ɐݒ�");
                }

                // �v���C���[�������iSpawnPoint�����Ńv���n�u�ʒu�g�p�j
                StartCoroutine(InitializePlayerAfterSceneLoad());
            };

            yield break; // �ʏ�� Additive �S���[�h�������X�L�b�v
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
                SetSceneActive(name, name == "Title"); // �N������Title����ON
            }
        }

        Debug.Log("[BootLoader] ���ׂẴV�[�������O���[�h�����BTitle�\�����B");

#if UNITY_EDITOR
        // �f�o�b�O�X�L�b�v�L���Ȃ�^�C�g���𖳌������Ďw��V�[����L����
        if (enableDebugStart && !string.IsNullOrEmpty(debugSceneName))
        {
            yield return new WaitForSeconds(waitForBoot);
            DebugSkipToScene(debugSceneName);
        }
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// �f�o�b�O�X�L�b�v�Ŏw��V�[����L��������
    /// </summary>
    private void DebugSkipToScene(string sceneName)
    {
        if (!loadedScenes.ContainsKey(sceneName))
        {
            Debug.LogWarning($"[BootLoader] �f�o�b�O�V�[�� '{sceneName}' ���v�����[�h����Ă��܂���BpreloadScenes �ɒǉ����Ă��������B");
            return;
        }

        Debug.Log($"[BootLoader]  �f�o�b�O���[�h�L��: Title���X�L�b�v���� '{sceneName}' ���A�N�e�B�u�����܂��B");

        // Title��OFF
        SetSceneActive("Title", false);

        // �ΏۃV�[����ON
        SetSceneActive(sceneName, true);

        // �A�N�e�B�u�V�[���ݒ�
        var scene = loadedScenes[sceneName];
        SceneManager.SetActiveScene(scene);

        // �v���C���[������
        StartCoroutine(InitializePlayerAfterSceneLoad());
    }
#endif

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

        // --- ������ǉ� ---
        Debug.Log("[BootLoader] �͂��߂���̂��ߐi�s�x�����Z�b�g���c");
        var triggers = Object.FindObjectsByType<ItemTrigger>(FindObjectsSortMode.None);
        foreach (var t in triggers)
        {
            t.LoadProgress(0);  // �i�s�x�������I��0�ɖ߂�
        }

        GameFlags.Instance?.ClearAllFlags(); // ���K�v�Ȃ�t���O�����Z�b�g

        // �v���C���[������
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
        // �f�o�b�O�X�L�b�v����SpawnPoint�𖳎����A�v���n�u���W���g�p
        if (enableDebugStart)
        {
            Debug.Log($"[BootLoader] �f�o�b�O�X�L�b�v���̂��߁ASpawnPoint�����B�v���n�u�������W {player.transform.position} ���g�p���܂��B");
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
            else
            {
                Debug.LogWarning("[BootLoader] SpawnPoint��������܂���ł����B�v���n�u�����ʒu���g�p���܂��B");
            }
        }

        var move = player.GetComponent<GridMovement>();
        if (move != null)
            move.SetDirection(0);

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