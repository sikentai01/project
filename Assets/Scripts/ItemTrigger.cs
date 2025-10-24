using System.Collections;
using System.IO;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    // ======== 元コード部分はそのまま維持 ========
    public string triggerID;
    public ItemData itemData;
    public GameObject targetObject;
    public int requiredDirection = -1;
    public bool canInspectAfterCollected = false;
    public string[] systemMessageFiles;
    public bool freezeDuringText = true;

    private bool isPlayerNear = false;
    private GridMovement playerMovement;
    private int currentStage = 0;
    private bool initialized = false;

    private void OnEnable()
    {
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return null; // BootLoaderやGameFlagsの初期化完了待ち

        if (GameFlags.Instance != null && GameFlags.Instance.HasFlag(triggerID))
        {
            currentStage = 1;
        }
        else
        {
            currentStage = 0;
        }

        UpdateVisualState(); // ←★ 見た目反映を確実に呼ぶ
        initialized = true;

        Debug.Log($"[ItemTrigger] 初期化完了: {triggerID}, currentStage={currentStage}");
    }

    void Update()
    {
        if (!initialized) return;
        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            if (requiredDirection != -1 && playerMovement != null &&
                playerMovement.GetDirection() != requiredDirection)
                return;

            if (currentStage == 0)
                CollectItem();
            else if (canInspectAfterCollected)
                StartCoroutine(PlaySystemTextRoutine());
        }
    }

    private void CollectItem()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            GameFlags.Instance?.SetFlag(triggerID);
            currentStage = 1;

            UpdateVisualState();
            StartCoroutine(PlaySystemTextRoutine());
        }
    }

    private IEnumerator PlaySystemTextRoutine()
    {
        if (freezeDuringText && playerMovement != null)
            playerMovement.enabled = false;

        // ★↓↓↓↓ここだけ書き換え↓↓↓↓
        foreach (var fileName in systemMessageFiles)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, "SystemWindow", fileName + ".txt");
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"[ItemTrigger] ファイルが存在しません: {fullPath}");
                continue;
            }

            string text = File.ReadAllText(fullPath, System.Text.Encoding.UTF8);

            var adapter = ConversationTriggerAdapter.Instance ??
                          Object.FindObjectOfType<ConversationTriggerAdapter>(true);

            if (adapter != null)
            {
                // “txtの中身を直接会話UIに送る”
                adapter.FireRawText(text, fileName, true);
                Debug.Log($"[ItemTrigger] SystemWindow テキスト {fileName} を表示");
            }
            else
            {
                Debug.LogWarning("[ItemTrigger] ConversationTriggerAdapter が見つかりません。");
            }

            // 会話が終わるまで待機（必要に応じて調整）
            yield return new WaitUntil(() => !IsConversationActive());
        }
        // ★↑↑↑↑ここだけ書き換え↑↑↑↑

        if (freezeDuringText && playerMovement != null)
            playerMovement.enabled = true;
    }

    private bool IsConversationActive()
    {
        var core = FindObjectOfType<DialogueCore>();
        return core != null && core.enabled;
    }

    private void UpdateVisualState()
    {
        if (targetObject == null) return;
        targetObject.SetActive(currentStage == 0 || canInspectAfterCollected);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            playerMovement = other.GetComponent<GridMovement>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            playerMovement = null;
        }
    }

    public ItemTriggerSaveData SaveProgress()
    {
        return new ItemTriggerSaveData
        {
            triggerID = this.triggerID,
            currentStage = this.currentStage
        };
    }

    public void LoadProgress(int stage)
    {
        this.currentStage = stage;
        UpdateVisualState();
    }
}

[System.Serializable]
public class ItemTriggerSaveData
{
    public string triggerID;
    public int currentStage;
}
