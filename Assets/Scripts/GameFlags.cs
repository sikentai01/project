using System.Collections.Generic;
using UnityEngine;

public class GameFlags : MonoBehaviour
{
    public static GameFlags Instance;

    // 現在のフラグを保持（例：アイテム取得やギミック進行）
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

    // ======================
    // フラグ操作系
    // ======================

    // フラグをセット
    public void SetFlag(string flagName)
    {
        if (!flags.Contains(flagName))
        {
            flags.Add(flagName);
            Debug.Log($"[GameFlags] フラグ追加: {flagName}");
        }
    }

    // フラグを持っているか確認
    public bool HasFlag(string flagName)
    {
        return flags.Contains(flagName);
    }

    // フラグ削除
    public void RemoveFlag(string flagName)
    {
        if (flags.Contains(flagName))
        {
            flags.Remove(flagName);
            Debug.Log($"[GameFlags] フラグ削除: {flagName}");
        }
    }

    // 全削除（初期化）
    public void ClearAllFlags()
    {
        flags.Clear();
        Debug.Log("[GameFlags] 全フラグをクリアしました");
    }

    // ======================
    //  セーブ・ロード用
    // ======================

    // セーブデータを作成
    public FlagSaveData SaveFlags()
    {
        var data = new FlagSaveData();
        data.activeFlags = new List<string>(flags).ToArray();
        Debug.Log($"[GameFlags] {data.activeFlags.Length} 件のフラグを保存しました");
        return data;
    }

    // セーブデータを読み込み
    public void LoadFlags(FlagSaveData data)
    {
        flags.Clear();

        if (data?.activeFlags != null)
        {
            foreach (string f in data.activeFlags)
            {
                flags.Add(f);
            }
            Debug.Log($"[GameFlags] {flags.Count} 件のフラグを復元しました");
        }
        else
        {
            Debug.LogWarning("[GameFlags] 復元するフラグデータがありません");
        }
    }
}

// =======================
// セーブデータ構造体
// =======================
[System.Serializable]
public class FlagSaveData
{
    public string[] activeFlags;
}
