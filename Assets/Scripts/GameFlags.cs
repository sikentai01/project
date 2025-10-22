using System.Collections.Generic;
using UnityEngine;

public class GameFlags : MonoBehaviour
{
    public static GameFlags Instance;

    private HashSet<string> flags = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetFlag(string flagName)
    {
        if (!flags.Contains(flagName))
        {
            flags.Add(flagName);
            Debug.Log($"[GameFlags] �t���O�ǉ�: {flagName}");
        }
    }

    public bool HasFlag(string flagName) => flags.Contains(flagName);

    public void RemoveFlag(string flagName)
    {
        if (flags.Contains(flagName))
        {
            flags.Remove(flagName);
            Debug.Log($"[GameFlags] �t���O�폜: {flagName}");
        }
    }

    public void ClearAllFlags()
    {
        flags.Clear();
        Debug.Log("[GameFlags] �S�t���O���N���A���܂���");
    }

    public FlagSaveData SaveFlags()
    {
        var data = new FlagSaveData();
        data.activeFlags = new List<string>(flags).ToArray();
        Debug.Log($"[GameFlags] {data.activeFlags.Length} ���̃t���O��ۑ����܂���");
        return data;
    }

    public void LoadFlags(FlagSaveData data)
    {
        flags.Clear();

        if (data?.activeFlags != null)
        {
            foreach (string f in data.activeFlags)
                flags.Add(f);

            Debug.Log($"[GameFlags] {flags.Count} ���̃t���O�𕜌����܂���");
        }
        else
        {
            Debug.LogWarning("[GameFlags] ��������t���O�f�[�^������܂���");
        }
    }
}

[System.Serializable]
public class FlagSaveData
{
    public string[] activeFlags;
}