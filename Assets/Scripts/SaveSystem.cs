using UnityEngine;
using System.IO;
using System.Collections.Generic; // [NEW]

public static class SaveSystem
{
    // ======================
    // セーブデータファイルのパス
    // ======================
    private static string GetSavePath(int slotNumber)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData_{slotNumber}.json");
    }

    // ======================
    // セーブ処理
    // ======================
    public static void SaveGame(int slotNumber, string sceneName, Vector3 playerPos, int playerDir)
    {
        var data = new SaveData
        {
            sceneName = sceneName,
            playerPosition = playerPos,
            playerDirection = playerDir,
            inventoryData = InventoryManager.Instance != null ? InventoryManager.Instance.SaveData() : new InventorySaveData(),
            documentData = DocumentManager.Instance != null ? DocumentManager.Instance.SaveData() : new DocumentSaveData(),
            flagData = GameFlags.Instance != null ? GameFlags.Instance.SaveFlags() : new FlagSaveData(),
            gimmickProgressList = new List<GimmickProgressData>() // [NEW]
        };

        // [NEW] シーン内の全 ItemTrigger から進行度を収集
        var triggers = Object.FindObjectsByType<ItemTrigger>(FindObjectsSortMode.None);
        foreach (var t in triggers)
        {
            if (!string.IsNullOrEmpty(t.triggerID))
            {
                data.gimmickProgressList.Add(t.SaveProgress());
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slotNumber), json);

        Debug.Log($"[SaveSystem] スロット{slotNumber}にセーブ完了！パス：{GetSavePath(slotNumber)}");
    }

    // ======================
    // ロード処理
    // ======================
    public static SaveData LoadGame(int slotNumber)
    {
        string path = GetSavePath(slotNumber);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] スロット{slotNumber}にセーブデータが存在しません。");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"[SaveSystem] スロット{slotNumber}からロード完了。シーン：{data.sceneName}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] ロード中にエラー: {e.Message}");
            return null;
        }
    }

    // ======================
    // セーブデータの存在確認
    // ======================
    public static bool DoesSaveExist(int slotNumber)
    {
        string savePath = Path.Combine(Application.persistentDataPath, $"saveData_{slotNumber}.json");
        return File.Exists(savePath);
    }

    // ======================
    // セーブデータ削除
    // ======================
    public static void DeleteSaveData(int slotNumber)
    {
        string path = GetSavePath(slotNumber);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveSystem] スロット{slotNumber}のセーブデータを削除しました。");
        }
        else
        {
            Debug.LogWarning($"[SaveSystem] スロット{slotNumber}に削除対象のセーブデータがありません。");
        }
    }

    // ======================
    // セーブデータ構造体
    // ======================
    [System.Serializable]
    public class SaveData
    {
        public string sceneName;
        public Vector3 playerPosition;
        public int playerDirection;
        public InventorySaveData inventoryData;
        public DocumentSaveData documentData;
        public FlagSaveData flagData;

        // ---- ここから追加 ----
        public List<GimmickProgressData> gimmickProgressList = new List<GimmickProgressData>(); // [NEW]
        // ----------------------
    }

    // [NEW] ギミック進行度1件分
    [System.Serializable]
    public class GimmickProgressData
    {
        public string triggerID;
        public int stage;
    }
}