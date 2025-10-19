using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BootScenePruner : MonoBehaviour
{
    [Header("起動時に残す部屋シーン名（例: Scenes0）")]
    [SerializeField] private string startRoomSceneName = "Scenes0";

    [Header("起動時の出現スポーン名（例: SpawnPoint）")]
    [SerializeField] private string startSpawnName = "SpawnPoint";

    private bool isStarting = false;

    void Awake()
    {
        Debug.Log("[BootPruner] 起動しました（タイトル経由モード：自動ロード無効）");
    }

    /// <summary>
    /// はじめからボタン等から呼び出して、ゲーム開始処理を実行する。
    /// </summary>
    public void StartGame()
    {
        Debug.Log("[BootPruner] StartGame 呼び出し → ゲーム開始");

        LevelSpawnRouter2D.HasPendingTeleport = false;
        LevelSpawnRouter2D.PendingSceneName = null;

        if (!string.IsNullOrEmpty(startSpawnName))
            LevelSpawnRouter2D.NextSpawnPointName = startSpawnName;

        //  Sceneが既にロード済みかチェック
        var existingScene = SceneManager.GetSceneByName(startRoomSceneName);
        if (existingScene.IsValid() && existingScene.isLoaded)
        {
            Debug.Log($"[BootPruner] {startRoomSceneName} は既にロード済み。アクティブ化のみ実行。");
            SceneManager.SetActiveScene(existingScene);
        }
        else
        {
            Debug.Log($"[BootPruner] {startRoomSceneName} をロードします…");
            SceneManager.LoadSceneAsync(startRoomSceneName, LoadSceneMode.Additive)
                .completed += _ =>
                {
                    var loaded = SceneManager.GetSceneByName(startRoomSceneName);
                    if (loaded.IsValid())
                    {
                        SceneManager.SetActiveScene(loaded);
                        Debug.Log($"[BootPruner] {startRoomSceneName} をアクティブシーンに設定");
                    }
                };
        }

        //  Titleをアンロード
        var titleScene = SceneManager.GetSceneByName("Title");
        if (titleScene.IsValid())
        {
            SceneManager.UnloadSceneAsync(titleScene);
            Debug.Log("[BootPruner] Title シーンをアンロードしました。");
        }
    }

    private IEnumerator LoadStartSceneRoutine()
    {
        if (string.IsNullOrEmpty(startRoomSceneName))
        {
            Debug.LogError("[BootPruner] startRoomSceneName が設定されていません！");
            yield break;
        }

        Debug.Log($"[BootPruner] {startRoomSceneName} をロード開始…");

        // Bootstrapにカメラや音などがある前提でAdditiveロード
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(startRoomSceneName, LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadOp.isDone);

        // ③ 読み込んだシーンをアクティブに
        var loaded = SceneManager.GetSceneByName(startRoomSceneName);
        if (loaded.IsValid())
        {
            SceneManager.SetActiveScene(loaded);
            Debug.Log($"[BootPruner] {startRoomSceneName} をアクティブシーンに設定完了");
        }
        else
        {
            Debug.LogError($"[BootPruner] シーン {startRoomSceneName} が読み込めませんでした。");
        }

        // ④ Titleシーンをアンロード（安全に）
        yield return new WaitForSeconds(0.2f); // 少し待ってから消すとカメラ破棄回避できる
        Scene titleScene = SceneManager.GetSceneByName("Title");
        if (titleScene.IsValid())
        {
            SceneManager.UnloadSceneAsync(titleScene);
            Debug.Log("[BootPruner] Title シーンをアンロードしました。");
        }

        Debug.Log("[BootPruner] ゲーム開始処理完了！");
    }
}