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

    [Header("拾った時に再生するサウンド（任意）")]
    public AudioClip pickupSE;

    private bool isPlayerNear = false;
    private GridMovement playerMovement;

    public bool IsPlayerNear => isPlayerNear;

    private void Start()
    {
        UpdateVisual();
    }

    private void Update()
    {
        if (PauseMenu.isPaused) return;
        if (SaveSlotUIManager.Instance != null && SaveSlotUIManager.Instance.IsOpen()) return;

        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // 向きチェック
            if (requiredDirection != -1 && playerMovement != null &&
                playerMovement.GetDirection() != requiredDirection)
                return;

            // 未取得のときのみ拾える
            if (currentStage == 0)
                CollectItem();
        }
    }

    // === アイテム入手処理 ===
    private void CollectItem()
    {
        if (itemData == null) return;

        // ① アイテム追加
        InventoryManager.Instance.AddItem(itemData);
        Debug.Log($"[ItemTrigger] {itemData.itemName} を入手！");

        // ② 状態更新
        currentStage = 1;
        UpdateVisual();

        // ③ 効果音再生（任意）
        if (SoundManager.Instance != null && pickupSE != null)
            SoundManager.Instance.PlaySE(pickupSE);

        // ④ アニメーション通知（GameFlagsは使わない）
        var reactions = FindObjectsByType<GimmickReactionTarget>(FindObjectsSortMode.None);
        foreach (var r in reactions)
        {
            r.ReactToTrigger(triggerID);
        }

        Debug.Log($"[ItemTrigger] {triggerID} 完了。進行度={currentStage}");
    }

    // === 見た目を切り替え ===
    private void UpdateVisual()
    {
        if (targetObject != null)
            targetObject.SetActive(currentStage == 0);
    }

    // === セーブ用データ作成 ===
    public SaveSystem.GimmickProgressData SaveProgress()
    {
        return new SaveSystem.GimmickProgressData
        {
            triggerID = triggerID,
            stage = currentStage
        };
    }

    // === ロード時に進行度を反映 ===
    public void LoadProgress(int stage)
    {
        currentStage = stage;
        UpdateVisual();
        Debug.Log($"[ItemTrigger] {triggerID} の進行度を {currentStage} に復元しました");
    }

    // === プレイヤー接触判定 ===
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
}