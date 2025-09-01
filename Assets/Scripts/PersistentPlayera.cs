using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentPlayer : MonoBehaviour
{
    // 同じプレイヤーが2体以上にならないようにする
    private static PersistentPlayer instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // シーン切り替え時に呼ばれるイベント登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 新しいシーンがロードされた直後に、SpawnPoint（あれば）へ移動
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var sp = GameObject.Find("SpawnPoint");
        if (sp != null) transform.position = sp.transform.position;
    }
}
