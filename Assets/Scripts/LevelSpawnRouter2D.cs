using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 扉遷移のみテレポ + Roomroot整理 + Reassert（最終適用）
/// ・ターゲット以外の OnSceneLoaded では Active を変更しない！
/// </summary>
public class LevelSpawnRouter2D : MonoBehaviour
{
    public static string NextSpawnPointName = "SpawnPoint";

    [Header("Roomroot名（大小無視・完全一致）")]
    [SerializeField] private string roomRootName = "Roomroot";

    // 扉遷移のフラグ
    public static bool   HasPendingTeleport = false;
    public static string PendingSceneName   = null;

    private static bool _reassertQueued = false;

    void OnEnable()  { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ★厳密ターゲット判定（扉から指定されたシーンだけ true）
        bool isTargetLoad = HasPendingTeleport
                            && !string.IsNullOrEmpty(PendingSceneName)
                            && scene.name == PendingSceneName;

        if (!isTargetLoad)
        {
            // ★ターゲット以外では Active を絶対に切り替えない！
            //   可視の乱れはフレーム末のReassertでまとめて正す
            QueueReassert();
            Debug.Log($"[SpawnRouter] Teleport SKIP: loaded='{scene.name}', pending={HasPendingTeleport}, target='{PendingSceneName}'");
            return;
        }

        // ここから正式遷移だけを処理
        // ① 遷移先をアクティブに（ターゲット時のみ！）
        SceneManager.SetActiveScene(scene);

        // ② プレイヤー取得
        var player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
        {
            Debug.LogWarning("[SpawnRouter] Player(tag=Player) が見つかりません。");
            QueueReassert();
            HasPendingTeleport = false;
            PendingSceneName   = null;
            return;
        }

        // ③ 新シーン内だけから Spawn を検索（大小無視）
        var spawnName = string.IsNullOrEmpty(NextSpawnPointName) ? "SpawnPoint" : NextSpawnPointName;
        var spawn = FindInSceneByName(scene, spawnName, ignoreCase: true);

        // ④ 安全ワープ（Rigidbody2D対応＋Z=0固定）
        if (spawn)
        {
            var rb = player.GetComponent<Rigidbody2D>();
            var target = spawn.transform.position; target.z = 0f;

#if UNITY_6000_0_OR_NEWER
            if (rb) { rb.linearVelocity = Vector2.zero; rb.simulated = false; rb.position = (Vector2)target; rb.simulated = true; }
#else
            if (rb) { rb.velocity = Vector2.zero; rb.simulated = false; rb.position = (Vector2)target; rb.simulated = true; }
#endif
            else { player.transform.position = target; }

            var p = player.transform.position;
            if (p.z != 0f) { p.z = 0f; player.transform.position = p; }
        }
        else
        {
            Debug.LogWarning($"[SpawnRouter] '{scene.name}' に Spawn '{spawnName}' が見つかりません。位置は現状維持。");
        }

        // ⑤ 一度即時で整理（体感のチラつき低減）
        ToggleRoomrootsForActiveScene(scene);

        // ⑥ フレーム末でもう一度最終適用（順序ゆらぎ対策）
        QueueReassert();

        // ⑦ フラグを落とす
        HasPendingTeleport = false;
        PendingSceneName   = null;

        Debug.Log($"[SpawnRouter] Active={scene.name}, Spawn='{spawnName}' → Teleport & Roomroot整理完了");
    }

    // ── Reassert（フレーム末に一回だけ最終ON/OFFを確定） ──
    private void QueueReassert()
    {
        if (_reassertQueued) return;
        _reassertQueued = true;
        StartCoroutine(ReassertAtEndOfFrame());
    }

    private IEnumerator ReassertAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        // Pending が残っていればそれ、無ければ現在のActiveを採用
        Scene target = SceneManager.GetActiveScene();
        if (!string.IsNullOrEmpty(PendingSceneName))
        {
            var s = SceneManager.GetSceneByName(PendingSceneName);
            if (s.IsValid() && s.isLoaded) target = s;
        }

        ToggleRoomrootsForActiveScene(target);
        _reassertQueued = false;
        // Debug.Log($"[SpawnRouter] Reassert → {target.name}");
    }

    // ── Roomroot ON/OFF ──
    private void ToggleRoomrootsForActiveScene(Scene active)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var s = SceneManager.GetSceneAt(i);
            if (!s.isLoaded) continue;
            bool shouldEnable = (s == active);
            foreach (var rr in FindAllNamedInScene(s, roomRootName))
                if (rr.activeSelf != shouldEnable) rr.SetActive(shouldEnable);
        }
    }

    // ── シーン内限定の名前検索（大小無視・再帰） ──
    private static GameObject FindInSceneByName(Scene scene, string name, bool ignoreCase)
    {
        if (!scene.IsValid() || !scene.isLoaded || string.IsNullOrEmpty(name)) return null;
        foreach (var root in scene.GetRootGameObjects())
        {
            var found = FindRecursive(root.transform, name, ignoreCase);
            if (found) return found;
        }
        return null;
    }

    private static GameObject FindRecursive(Transform t, string name, bool ignoreCase)
    {
        bool match = ignoreCase ? string.Equals(t.name, name, System.StringComparison.OrdinalIgnoreCase)
                                : t.name == name;
        if (match) return t.gameObject;

        for (int i = 0; i < t.childCount; i++)
        {
            var r = FindRecursive(t.GetChild(i), name, ignoreCase);
            if (r) return r;
        }
        return null;
    }

    private IEnumerable<GameObject> FindAllNamedInScene(Scene scene, string name)
    {
        var list = new List<GameObject>();
        foreach (var root in scene.GetRootGameObjects())
            CollectByName(root.transform, name, list);
        return list;
    }

    private static void CollectByName(Transform t, string name, List<GameObject> list)
    {
        if (!string.IsNullOrEmpty(name) &&
            string.Equals(t.gameObject.name, name, System.StringComparison.OrdinalIgnoreCase))
        {
            list.Add(t.gameObject);
        }
        for (int i = 0; i < t.childCount; i++)
            CollectByName(t.GetChild(i), name, list);
    }
}
