using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-500)]
public class ActiveSceneOnly : MonoBehaviour
{
    void OnEnable()
    {
        Apply();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }
    void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }
    void OnActiveSceneChanged(Scene oldS, Scene newS) => Apply();

    void Apply()
    {
        bool shouldEnable = gameObject.scene == SceneManager.GetActiveScene();
        if (gameObject.activeSelf != shouldEnable) gameObject.SetActive(shouldEnable);
    }
}
