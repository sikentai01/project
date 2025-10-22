using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string GetSavePath(int slotNumber)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData_{slotNumber}.json");
    }

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
            gimmickProgressList = new List<GimmickSaveData>(),
            itemTriggerList = new List<ItemTriggerSaveData>()
        };

        // === �M�~�b�N�i�s�x ===
        var allGimmicks = Resources.FindObjectsOfTypeAll<GimmickBase>();
        foreach (var g in allGimmicks)
        {
            // Prefab�A�Z�b�g�▢���[�h�V�[�������O
            if (string.IsNullOrEmpty(g.gimmickID)) continue;
            if (g.gameObject.scene == null || !g.gameObject.scene.isLoaded) continue;

            data.gimmickProgressList.Add(g.SaveProgress());
        }

        // === �A�C�e���g���K�[�i�s�x ===
        var allTriggers = Resources.FindObjectsOfTypeAll<ItemTrigger>();
        foreach (var t in allTriggers)
        {
            if (string.IsNullOrEmpty(t.triggerID)) continue;
            if (t.gameObject.scene == null || !t.gameObject.scene.isLoaded) continue;

            data.itemTriggerList.Add(t.SaveProgress());
        }

        // === JSON�o�� ===
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(slotNumber), json);
        Debug.Log($"[SaveSystem] �Z�[�u����: �M�~�b�N={data.gimmickProgressList.Count}, �A�C�e��={data.itemTriggerList.Count}");
    }

    public static SaveData LoadGame(int slotNumber)
    {
        string path = GetSavePath(slotNumber);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] �X���b�g{slotNumber}�ɃZ�[�u�f�[�^�Ȃ�");
            return null;
        }

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<SaveData>(json);
    }

    [System.Serializable]
    public class SaveData
    {
        public string sceneName;
        public Vector3 playerPosition;
        public int playerDirection;
        public InventorySaveData inventoryData;
        public DocumentSaveData documentData;
        public FlagSaveData flagData;

        public List<GimmickSaveData> gimmickProgressList = new List<GimmickSaveData>();
        public List<ItemTriggerSaveData> itemTriggerList = new List<ItemTriggerSaveData>();
    }
}