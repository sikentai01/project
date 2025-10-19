using UnityEngine;

public class DiarySavePoint : MonoBehaviour
{
    [Header("サウンド設定")]
    [SerializeField] private AudioClip saveSeClip;

    [Header("演出")]
    [SerializeField] private GameObject saveEffect;  // 光るエフェクトなど（任意）

    private bool isPlayerNear = false;
    private GridMovement player;

    void Start()
    {
        player = FindFirstObjectByType<GridMovement>();
        if (saveEffect != null)
            saveEffect.SetActive(false);
    }

    void Update()
    {
        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        // プレイヤーが近くにいて Enterキー が押されたらセーブスロットUIを開く
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            OpenSaveUI();
        }
    }

    private void OpenSaveUI()
    {
        if (SoundManager.Instance != null && saveSeClip != null)
            SoundManager.Instance.PlaySE(saveSeClip);

        if (saveEffect != null)
        {
            saveEffect.SetActive(true);
            Invoke(nameof(StopEffect), 1.0f);
        }

        // プレイヤーの動きを止める
        if (player != null)
            player.enabled = false;

        // セーブスロットUIを開く
        if (SaveSlotUIManager.Instance != null)
            SaveSlotUIManager.Instance.OpenSavePanel();
    }

    private void StopEffect()
    {
        if (saveEffect != null)
            saveEffect.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = false;
    }
}
