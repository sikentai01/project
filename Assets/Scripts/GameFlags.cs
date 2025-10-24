using System.Collections.Generic;
using UnityEngine;

public class GameFlags : MonoBehaviour
{
    public static GameFlags Instance;

    private HashSet<string> flags = new HashSet<string>();

    // ★ 追加: float値を保存する辞書
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
    // 既存のフラグ制御
    // =============================
    public void SetFlag(string flagName)
    {
        if (!flags.Contains(flagName))
        {
            flags.Add(flagName);
            Debug.Log($"[GameFlags] フラグ追加: {flagName}");
        }
    }

    public bool HasFlag(string flagName) => flags.Contains(flagName);

    public void RemoveFlag(string flagName)
    {
        if (flags.Contains(flagName))
        {
            flags.Remove(flagName);
            Debug.Log($"[GameFlags] フラグ削除: {flagName}");
        }
    }

    public void ClearAllFlags()
    {
        flags.Clear();
        floatValues.Clear(); // 数値も消す
        Debug.Log("[GameFlags] 全フラグをクリアしました");
    }

    // =============================
    // 数値の管理用（★追加）
    // =============================
    public void SetFloat(string key, float value)
    {
        floatValues[key] = value;
        Debug.Log($"[GameFlags] float登録: {key} = {value}");
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
    // セーブ/ロード
    // =============================
    public FlagSaveData SaveFlags()
    {
        var data = new FlagSaveData();
        data.activeFlags = new List<string>(flags).ToArray();
        data.floatKeys = new List<string>(floatValues.Keys).ToArray();
        data.floatValues = new List<float>(floatValues.Values).ToArray();
        Debug.Log($"[GameFlags] フラグ={flags.Count}, 数値={floatValues.Count} 件を保存しました");
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

            Debug.Log($"[GameFlags] フラグ={flags.Count}, 数値={floatValues.Count} 件を復元しました");
        }
        else
        {
            Debug.LogWarning("[GameFlags] 復元データなし");
        }
    }
}

[System.Serializable]
public class FlagSaveData
{
    public string[] activeFlags;
    public string[] floatKeys;   // ★ 追加
    public float[] floatValues;  // ★ 追加
}
