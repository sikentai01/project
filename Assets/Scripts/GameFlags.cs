using System.Collections.Generic;
using UnityEngine;

public class GameFlags : MonoBehaviour
{
    public static GameFlags Instance;

    private HashSet<string> flags = new HashSet<string>();

    // �� �ǉ�: float�l��ۑ����鎫��
    private Dictionary<string, float> floatValues = new Dictionary<string, float>();

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

    // =============================
    // �����̃t���O����
    // =============================
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
        floatValues.Clear(); // ���l������
        Debug.Log("[GameFlags] �S�t���O���N���A���܂���");
    }

    // =============================
    // ���l�̊Ǘ��p�i���ǉ��j
    // =============================
    public void SetFloat(string key, float value)
    {
        floatValues[key] = value;
        Debug.Log($"[GameFlags] float�o�^: {key} = {value}");
    }

    public float GetFloat(string key)
    {
        return floatValues.ContainsKey(key) ? floatValues[key] : 0f;
    }

    public void AddFloat(string key, float delta)
    {
        if (!floatValues.ContainsKey(key))
            floatValues[key] = 0f;
        floatValues[key] += delta;
    }

    // =============================
    // �Z�[�u/���[�h
    // =============================
    public FlagSaveData SaveFlags()
    {
        var data = new FlagSaveData();
        data.activeFlags = new List<string>(flags).ToArray();
        data.floatKeys = new List<string>(floatValues.Keys).ToArray();
        data.floatValues = new List<float>(floatValues.Values).ToArray();
        Debug.Log($"[GameFlags] �t���O={flags.Count}, ���l={floatValues.Count} ����ۑ����܂���");
        return data;
    }

    public void LoadFlags(FlagSaveData data)
    {
        flags.Clear();
        floatValues.Clear();

        if (data != null)
        {
            if (data.activeFlags != null)
            {
                foreach (string f in data.activeFlags)
                    flags.Add(f);
            }

            if (data.floatKeys != null && data.floatValues != null)
            {
                for (int i = 0; i < Mathf.Min(data.floatKeys.Length, data.floatValues.Length); i++)
                    floatValues[data.floatKeys[i]] = data.floatValues[i];
            }

            Debug.Log($"[GameFlags] �t���O={flags.Count}, ���l={floatValues.Count} ���𕜌����܂���");
        }
        else
        {
            Debug.LogWarning("[GameFlags] �����f�[�^�Ȃ�");
        }
    }
}

[System.Serializable]
public class FlagSaveData
{
    public string[] activeFlags;
    public string[] floatKeys;   // �� �ǉ�
    public float[] floatValues;  // �� �ǉ�
}
