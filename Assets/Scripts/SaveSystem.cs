using UnityEngine;
using System.IO;
using System.Collections.Generic; // [NEW]

public static class SaveSystem
{
    // ======================
    // �Z�[�u�f�[�^�t�@�C���̃p�X
    // ======================
    private static string GetSavePath(int slotNumber)
    {
        return Path.Combine(Application.persistentDataPath, $"saveData_{slotNumber}.json");
    }

    // ======================
    // �Z�[�u����
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

        // [NEW] �V�[�����̑S ItemTrigger ����i�s�x�����W
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

        Debug.Log($"[SaveSystem] �X���b�g{slotNumber}�ɃZ�[�u�����I�p�X�F{GetSavePath(slotNumber)}");
    }

    // ======================
    // ���[�h����
    // ======================
    public static SaveData LoadGame(int slotNumber)
    {
        string path = GetSavePath(slotNumber);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[SaveSystem] �X���b�g{slotNumber}�ɃZ�[�u�f�[�^�����݂��܂���B");
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"[SaveSystem] �X���b�g{slotNumber}���烍�[�h�����B�V�[���F{data.sceneName}");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SaveSystem] ���[�h���ɃG���[: {e.Message}");
            return null;
        }
    }

    // ======================
    // �Z�[�u�f�[�^�̑��݊m�F
    // ======================
    public static bool DoesSaveExist(int slotNumber)
    {
        string savePath = Path.Combine(Application.persistentDataPath, $"saveData_{slotNumber}.json");
        return File.Exists(savePath);
    }

    // ======================
    // �Z�[�u�f�[�^�폜
    // ======================
    public static void DeleteSaveData(int slotNumber)
    {
        string path = GetSavePath(slotNumber);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveSystem] �X���b�g{slotNumber}�̃Z�[�u�f�[�^���폜���܂����B");
        }
        else
        {
            Debug.LogWarning($"[SaveSystem] �X���b�g{slotNumber}�ɍ폜�Ώۂ̃Z�[�u�f�[�^������܂���B");
        }
    }

    // ======================
    // �Z�[�u�f�[�^�\����
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

        // ---- ��������ǉ� ----
        public List<GimmickProgressData> gimmickProgressList = new List<GimmickProgressData>(); // [NEW]
        // ----------------------
    }

    // [NEW] �M�~�b�N�i�s�x1����
    [System.Serializable]
    public class GimmickProgressData
    {
        public string triggerID;
        public int stage;
    }
}