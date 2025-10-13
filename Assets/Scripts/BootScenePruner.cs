using UnityEngine;
using UnityEngine.SceneManagement;

public class BootScenePruner : MonoBehaviour
{
    [Header("起動時に残す部屋シーン名（例: Scenes00）")]
    [SerializeField] private string startRoomSceneName = "";
    [Header("起動時の出現スポーン名（例: SpawnPoint）")]
    [SerializeField] private string startSpawnName = "SpawnPoint";

    void Awake()
    {
        // 🚫 起動時は「扉遷移」扱いにしない（SpawnRouter側でTeleportしないように）
        LevelSpawnRouter2D.HasPendingTeleport = false;
        LevelSpawnRouter2D.PendingSceneName   = null;

        var boot = gameObject.scene;
        Scene startRoom = default(Scene);
        bool hasStartRoomLoaded = false;

        // 現在ロード中のシーンを確認
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (!s.isLoaded) continue;
            if (s == boot) continue;

            // 指定の開始シーンが既に開かれているかチェック
            if (!string.IsNullOrEmpty(startRoomSceneName) && s.name == startRoomSceneName)
            {
                startRoom = s;
                hasStartRoomLoaded = true;
            }
            else
            {
                // 余分なシーンをアンロード
                SceneManager.UnloadSceneAsync(s);
            }
        }

        // Spawnポイント名を設定（念のため）
        if (!string.IsNullOrEmpty(startSpawnName))
            LevelSpawnRouter2D.NextSpawnPointName = startSpawnName;

        // 開始シーンをロードまたはActive化
        if (!string.IsNullOrEmpty(startRoomSceneName))
        {
            if (hasStartRoomLoaded)
            {
                SceneManager.SetActiveScene(startRoom);
            }
            else
            {
                SceneManager.LoadSceneAsync(startRoomSceneName, LoadSceneMode.Additive)
                    .completed += _ =>
                    {
                        var loaded = SceneManager.GetSceneByName(startRoomSceneName);
                        if (loaded.IsValid())
                            SceneManager.SetActiveScene(loaded);
                    };
            }
        }

        Debug.Log($"[BootPruner] 起動整理完了: startRoom='{startRoomSceneName}', spawn='{startSpawnName}'");
    }
}
