using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DebugBootstrap : MonoBehaviour
{
    [Header("デバッグ起動シーン名")]
    public string targetSceneName = "Scenes01";

    void Awake()
    {
        if (FindObjectOfType<BootLoader>() != null)
            return;

        DebugSettings.EnableDebugMode = true;
        Debug.Log("[DebugBootstrap] BootLoader未検出 → デバッグモードON");

        EnsureSystemObjectsExist();
        EnsurePlayerAndCameraExist();

        if (!string.IsNullOrEmpty(targetSceneName))
        {
            Scene current = SceneManager.GetActiveScene();
            if (current.name != targetSceneName)
                SceneManager.LoadScene(targetSceneName, LoadSceneMode.Single);
        }
    }

    private void EnsureSystemObjectsExist()
    {
        if (SoundManager.Instance == null)
            new GameObject("SoundManager").AddComponent<SoundManager>();

        if (InventoryManager.Instance == null)
            new GameObject("InventoryManager").AddComponent<InventoryManager>();

        if (DocumentManager.Instance == null)
            new GameObject("DocumentManager").AddComponent<DocumentManager>();

        if (GameFlags.Instance == null)
            new GameObject("GameFlags").AddComponent<GameFlags>();

        if (PauseMenu.Instance == null)
            new GameObject("PauseMenu").AddComponent<PauseMenu>();

        Debug.Log("[DebugBootstrap] 各マネージャーを自動生成しました");
    }

    private void EnsurePlayerAndCameraExist()
    {
        // --- Player ---
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            var prefab = Resources.Load<GameObject>("Player");
            if (prefab != null)
            {
                player = GameObject.Instantiate(prefab);
                player.name = "Player (Debug)";
                Debug.Log("[DebugBootstrap] Playerを自動生成しました");
            }
        }

        // --- Camera ---
        var cam = Camera.main;
        if (cam == null)
        {
            var camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.orthographic = true;
            cam.transform.position = new Vector3(0, 0, -10);
            camObj.AddComponent<AudioListener>();
            Debug.Log("[DebugBootstrap] カメラを自動生成しました");
        }

        // --- Light2D ---
        var light = GameObject.FindObjectOfType<Light2D>();
        if (light == null)
        {
            var lightObj = new GameObject("Global Light 2D");
            light = lightObj.AddComponent<Light2D>();
            light.lightType = Light2D.LightType.Global;
            light.intensity = 0.5f;
            Debug.Log("[DebugBootstrap] Global Light 2D を自動生成しました");
        }
    }
}