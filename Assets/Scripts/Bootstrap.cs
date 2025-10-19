using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Bootstrap : MonoBehaviour
{
    private static Bootstrap _instance;

    // ドメインリロード無効対策
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        _instance = null;
        _ddolIds?.Clear();
    }

    private static HashSet<int> _ddolIds = new HashSet<int>();

    void Awake()
    {
        // ① すでにインスタンスがあるなら削除
        if (_instance != null && _instance != this)
        {
            Debug.Log("[Bootstrap] 重複検出 → 削除");
            Destroy(gameObject);
            return;
        }

        _instance = this;

        // ② 自分が Prefab の子ならスキップ（内側Bootstrap対策）
        if (transform.parent != null && transform.parent.name == "Bootstrap")
        {
            Debug.Log("[Bootstrap] 子Bootstrap検出 → 処理スキップ");
            return;
        }

        // ③ すでにDDOLシーンに登録済みならスキップ
        var root = transform.root.gameObject;
        int id = root.GetInstanceID();
        if (root.scene.name == "DontDestroyOnLoad" || _ddolIds.Contains(id))
        {
            Debug.Log("[Bootstrap] 既にDDOL済み → スキップ");
            return;
        }

        // ④ 一度だけ登録
        DontDestroyOnLoad(root);
        _ddolIds.Add(id);
        Debug.Log("[Bootstrap] 登録完了 → DontDestroyOnLoad適用");
    }
}