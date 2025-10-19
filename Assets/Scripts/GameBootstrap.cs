using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    public static SaveSystem.SaveData loadedData;

    private void Start()
    {
        if (loadedData == null)
        {
            Debug.Log("[GameBootstrap] ロードデータが存在しません。通常起動を続行。");
            return;
        }

        Debug.Log("[GameBootstrap] ロードデータ適用開始");

        // BootLoaderを取得
        var boot = FindFirstObjectByType<BootLoader>();
        if (boot == null)
        {
            Debug.LogError("[GameBootstrap] BootLoaderが見つかりません！");
            return;
        }

        // Titleを非表示にし、ロード対象シーンを有効化
        foreach (var kv in boot.loadedScenes)
            boot.SetSceneActive(kv.Key, kv.Key == loadedData.sceneName);

        // アクティブシーン変更
        var targetScene = SceneManager.GetSceneByName(loadedData.sceneName);
        if (targetScene.IsValid())
            SceneManager.SetActiveScene(targetScene);

        // === データ反映 ===
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = loadedData.playerPosition;
            var move = player.GetComponent<GridMovement>();
            if (move != null)
            {
                move.SetDirection(loadedData.playerDirection);
                move.enabled = true;
            }
        }

        if (InventoryManager.Instance != null && loadedData.inventoryData != null)
            InventoryManager.Instance.LoadData(loadedData.inventoryData);
        if (DocumentManager.Instance != null && loadedData.documentData != null)
            DocumentManager.Instance.LoadData(loadedData.documentData);
        if (GameFlags.Instance != null && loadedData.flagData != null)
            GameFlags.Instance.LoadFlags(loadedData.flagData);

        TitleManager.isTitleActive = false;
        PauseMenu.blockMenu = false;
        GameOverController.isGameOver = false;

        Debug.Log($"[GameBootstrap] {loadedData.sceneName} の状態を復元しました。");

        loadedData = null;
    }
}