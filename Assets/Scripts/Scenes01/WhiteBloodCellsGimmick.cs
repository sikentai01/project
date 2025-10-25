using System.Collections;
using UnityEngine;
// using UnityEngine.SceneManagement; // BootLoader���������̃t�H�[���o�b�N�Ŏg���Ȃ�

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
        _col.isTrigger = true; // 㩂Ȃ̂Ńg���K�[��OK�i�ǂɂ������ꍇ�͊O�Őؑցj
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

        // BootLoader ������Ȃ炱����
        if (BootLoader.Instance != null)
        {
            BootLoader.Instance.SwitchSceneInstant("GameOver");
        }
        else
        {
            // �� BootLoader�������v���W�F�N�g�Ȃ炱��ɍ����ւ�
            // SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }
    }
}
