using UnityEngine;

public class SceneBGMController : MonoBehaviour
{
    [Header("このシーン専用のBGM")]
    [SerializeField] private AudioClip sceneBGM;

    private bool isPlaying = false;

    private void OnEnable()
    {
        // BootLoaderでシーンが有効化された時（GameObjectがSetActive(true)になった）
        if (sceneBGM != null)
        {
            Debug.Log($"[SceneBGMController] シーン有効化 → BGM再生開始: {sceneBGM.name}");
            SoundManager.Instance?.StopBGM();
            SoundManager.Instance?.PlayBGM(sceneBGM);
            isPlaying = true;
        }
        else
        {
            Debug.LogWarning($"[SceneBGMController] BGM未設定: {gameObject.scene.name}");
        }
    }

    private void OnDisable()
    {
        // BootLoaderでシーンが非アクティブ化された時（SetActive(false)）
        if (isPlaying)
        {
            Debug.Log($"[SceneBGMController] シーン無効化 → BGM停止: {gameObject.scene.name}");
            SoundManager.Instance?.StopBGM();
            isPlaying = false;
        }
    }
}
