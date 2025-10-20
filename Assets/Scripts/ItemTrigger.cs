using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    [Header("このトリガー固有のID（セーブ用にユニークに設定）")]
    public string triggerID;

    [Header("拾えるアイテムデータ")]
    public ItemData itemData;

    [Header("見た目を消すオブジェクト（任意）")]
    public GameObject targetObject;

    [Header("必要な向き (0=下,1=左,2=右,3=上, -1=制限なし)")]
    public int requiredDirection = -1;

    [Header("現在の進行度 (0=未取得, 1=取得済)")]
    public int currentStage = 0;

    private bool isPlayerNear = false;
    private GridMovement playerMovement;

    public bool IsPlayerNear => isPlayerNear;

    private void Start()
    {
        // --- ロード時に見た目を復元 ---
        if (currentStage == 0)
        {
            if (targetObject != null)
                targetObject.SetActive(true);
        }
        else
        {
            if (targetObject != null)
                targetObject.SetActive(false);
        }
    }

    void Update()
    {
        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // 向きチェック
            if (requiredDirection != -1 && playerMovement != null && playerMovement.GetDirection() != requiredDirection)
            {
                Debug.Log("方向が合っていないのでアイテムを取得できない");
                return;
            }

            // 未取得のときのみ拾える
            if (currentStage == 0)
                CollectItem();
        }
    }

    // --- アイテム入手処理 ---
    public void CollectItem()
    {
        if (itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData);
            Debug.Log(itemData.itemName + " を入手！");

            currentStage = 1; // 拾ったので進行度更新

            if (targetObject != null)
                targetObject.SetActive(false);
            else
                gameObject.SetActive(false);

            // セーブ連動するならここでGameFlagsに登録も可
            GameFlags.Instance?.SetFlag(triggerID);
        }
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

    // =======================
    //  セーブ／ロード対応追加
    // =======================
    public SaveSystem.GimmickProgressData SaveProgress()
    {
        return new SaveSystem.GimmickProgressData
        {
            triggerID = this.triggerID,
            stage = this.currentStage
        };
    }

    public void LoadProgress(int savedStage)
    {
        currentStage = savedStage;

        // --- 見た目を進行度に合わせて復元 ---
        if (currentStage == 0)
        {
            if (targetObject != null)
                targetObject.SetActive(true);
        }
        else
        {
            if (targetObject != null)
                targetObject.SetActive(false);
        }

        Debug.Log($"[ItemTrigger] {triggerID} の進行度を {currentStage} に復元しました。");
    }
}