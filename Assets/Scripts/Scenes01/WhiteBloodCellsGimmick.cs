using System.Collections;
using UnityEngine;
// using UnityEngine.SceneManagement; // BootLoaderが無い時のフォールバックで使うなら

[RequireComponent(typeof(Collider2D))]
public class WhiteBloodGimmic : MonoBehaviour
{
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private string playerTag = "Player";

    private bool _fired;
    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true; // 罠なのでトリガーでOK（壁にしたい場合は外で切替）
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_fired) return;
        if (!other.CompareTag(playerTag)) return;

        _fired = true;
        StartCoroutine(Co_GameOverAfterDelay());
    }

    private IEnumerator Co_GameOverAfterDelay()
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        // BootLoader があるならこちら
        if (BootLoader.Instance != null)
        {
            BootLoader.Instance.SwitchSceneInstant("GameOver");
        }
        else
        {
            // ★ BootLoaderが無いプロジェクトならこれに差し替え
            // SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }
    }
}
