#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Bootstrapシーンと作業シーンを同時に開いているとき、
/// 実行開始時にダミー（編集中のシーンの内容）を自動で非アクティブ化して、
/// BootLoaderがAdditiveでロードする本物のシーンと重複しないようにする。
/// 実行停止後は DebugSceneAutoRestore.cs が自動で再アクティブ化する。
/// </summary>
[ExecuteAlways]
public class DebugSceneHelper : MonoBehaviour
{
    [Header("ヒエラルキー上に一緒に開いている作業シーン名を登録")]
    [SerializeField] private string[] sceneNames = { "Scenes0", "Scenes01", "Scenes02" };

    private void Awake()
    {
        if (Application.isPlaying)
        {
            foreach (var name in sceneNames)
            {
                var dummy = GameObject.Find(name);
                if (dummy != null)
                {
                    dummy.SetActive(false);
                    Debug.Log($"[DebugSceneHelper] 実行開始: {name} を一時的に非アクティブ化しました。");
                }
            }
        }
    }
}
#endif