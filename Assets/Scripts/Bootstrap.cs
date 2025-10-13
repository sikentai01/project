using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Bootstrap : MonoBehaviour
{
    private static Bootstrap _instance;

    // ドメインリロード無効対策：再生ごとに static を初期化
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        _instance = null;
        _ddolIds?.Clear();
    }

    // 既にDDOLへ移したルートのInstanceIDを共有管理（重複防止）
    private static HashSet<int> _ddolIds = new HashSet<int>();

    void Awake()
    {
        // ① シングルトン：重複Bootstrapは自滅
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        // ② ルートに対してのみ処理する（子では呼ばない）
        var root = transform.root.gameObject;
        int id = root.GetInstanceID();

        // ③ 既にDDOLシーンにいる or 他所で登録済みならスキップ
        if (root.scene.name == "DontDestroyOnLoad" || _ddolIds.Contains(id))
            return;

        // ④ 一度だけDDOL
        DontDestroyOnLoad(root);
        _ddolIds.Add(id);
    }
}
