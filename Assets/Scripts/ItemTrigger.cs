using System.Collections;
using System.IO;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [Header("このトリガー固有のID（GameFlagsに登録）")]
    public string triggerID;

    [Header("拾えるアイテムデータ")]
    public ItemData itemData;

    [Header("見た目を消すオブジェクト（任意）")]
    public GameObject targetObject;

    [Header("必要な向き (0=下,1=左,2=右,3=上, -1=制限なし)")]
    public int requiredDirection = -1;

    [Header("拾った後も調べられる？")]
    public bool canInspectAfterCollected = false;

    [Header("システムメッセージファイル名（StreamingAssets/SystemWindow以下）")]
    public string[] systemMessageFiles;

    [Header("テキスト再生中にプレイヤーを停止するか")]
    public bool freezeDuringText = true;

    private bool isPlayerNear = false;
    private GridMovement playerMovement;
    private int currentStage = 0;

    void Start()
    {
        // === フラグによる状態復元 ===
        if (GameFlags.Instance != null && GameFlags.Instance.HasFlag(triggerID))
        {
            currentStage = 1;
            UpdateVisualState();
        }
        else
        {
            currentStage = 0;
            UpdateVisualState();
        }
    }

    void Update()
    {
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

        foreach (var fileName in systemMessageFiles)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, "SystemWindow", fileName + ".txt");
            if (!File.Exists(fullPath)) continue;

            string text = File.ReadAllText(fullPath);
            DialogueCore.Instance?.StartConversation(fileName, text);
            yield return new WaitUntil(() => !IsConversationActive());
        }

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

    // ==========================================
    //  セーブ用データ構造と復元用メソッド
    // ==========================================
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