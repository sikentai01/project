using UnityEngine;
using System.Collections;

/// <summary>
/// 共通ドアコントローラー（セーブ・ロード・鍵使用対応）
/// stage:
///   0 = 閉
///   1 = 開（解錠済み）
/// </summary>
public class DoorController : GimmickBase
{
    public enum DoorMoveType { SameScene, ChangeScene }

    [Header("移動設定")]
    [SerializeField] private DoorMoveType moveType = DoorMoveType.SameScene;
    [SerializeField] private string targetScene = "";
    [SerializeField] private string targetPointName = "SpawnPoint";
    [SerializeField] private Transform targetPoint;

    [Header("ギミック情報")]
    public string gimmickID = "";
    public int currentStage = 0;

    [Header("鍵設定")]
    [Tooltip("必要な鍵アイテムID（空なら常時開放）")]
    [SerializeField] private string requiredKeyID = "";
    public int RequiredDirection => requiredDirection;

    [Header("開ける方向 (0=下,1=左,2=右,3=上)")]
    [SerializeField] private int requiredDirection = 0;

    [Header("演出")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private AudioClip openSE;
    [SerializeField] private AudioClip lockedSE;
    [SerializeField] private AudioClip unlockSE;

    private Transform player;
    private GridMovement playerMove;
    private bool isPlayerNear = false;
    private bool isProcessing = false;

    // 🔸 プレイヤーが近くにいるドアを静的に保持（KeyEffectで参照する）
    public static DoorController PlayerNearbyDoor;

    public string GetRequiredKeyID() => requiredKeyID;

    // =====================================================
    // トリガー検出
    // =====================================================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = true;
        player = other.transform;
        playerMove = player.GetComponent<GridMovement>();

        PlayerNearbyDoor = this; // ✅ 登録
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = false;
        player = null;
        playerMove = null;

        if (PlayerNearbyDoor == this)
            PlayerNearbyDoor = null; // ✅ 離れたら解除
    }

    // =====================================================
    // Update（方向一致時の入力検出）
    // =====================================================
    private void Update()
    {
        if (!isPlayerNear || playerMove == null || isProcessing) return;

        if (CheckInputDirection())
        {
            if (currentStage == 0)
            {
                Debug.Log("[Door] 鍵がかかっている");
                if (lockedSE) SoundManager.Instance?.PlaySE(lockedSE);

                if (doorAnimator != null)
                    doorAnimator.SetTrigger("Locked");
                else
                    Debug.Log("[Door] アニメーター未設定（Locked演出スキップ）");
            }
            else
            {
                StartCoroutine(OpenDoorAndMove());
            }
        }
    }

    // =====================================================
    // 入力方向チェック
    // =====================================================
    private bool CheckInputDirection()
    {
        bool pressed = false;
        int dir = -1;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) { dir = 0; pressed = true; }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) { dir = 1; pressed = true; }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) { dir = 2; pressed = true; }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) { dir = 3; pressed = true; }

        if (!pressed) return false;
        return dir == requiredDirection && playerMove.GetDirection() == requiredDirection;
    }

    // =====================================================
    // 鍵の使用処理
    // =====================================================
    public bool TryUseKey(string keyID)
    {
        // すでに開いてる場合はスキップ
        if (currentStage == 1)
        {
            Debug.Log($"[Door] {name} はすでに開いています");
            return false;
        }

        // 必要な鍵が一致しているか確認
        if (requiredKeyID != keyID)
        {
            Debug.Log($"[Door] {name} に {keyID} は合いません");
            return false;
        }

        // 開錠成功
        currentStage = 1;  // ← ★これが大事！
        Debug.Log($"[Door] 鍵 {keyID} で {name} を開錠しました");

        // アニメーションが設定されている場合
        if (doorAnimator != null)
            doorAnimator.SetTrigger("Open");
        else
            Debug.Log($"[Door] {name} にアニメーション未設定（即時開）");

        // 効果音再生（設定されていれば）
        if (unlockSE) SoundManager.Instance?.PlaySE(unlockSE);
        if (openSE) SoundManager.Instance?.PlaySE(openSE);

        // 進行度保存済みにする
        return true;
    }

    // =====================================================
    // ドアを開けて移動
    // =====================================================
    private IEnumerator OpenDoorAndMove()
    {
        isProcessing = true;
        yield return new WaitForSeconds(0.4f);

        if (openSE) SoundManager.Instance?.PlaySE(openSE);

        if (moveType == DoorMoveType.SameScene)
        {
            if (targetPoint != null && player != null)
            {
                player.position = targetPoint.position;
                Physics2D.SyncTransforms();
                Debug.Log($"[Door] {targetPoint.name} に移動完了");
            }
            else
            {
                Debug.LogWarning("[Door] targetPoint 未指定 or Player 不在");
            }
        }
        else
        {
            if (BootLoader.Instance != null)
                BootLoader.Instance.RequestSceneSwitch(targetScene, targetPointName);
            else
                Debug.LogWarning("[Door] BootLoader未設定のためシーン切替スキップ");
        }

        isProcessing = false;
    }

    // =====================================================
    // セーブ・ロード
    // =====================================================
    public override GimmickSaveData SaveProgress()
    {
        return new GimmickSaveData { gimmickID = gimmickID, stage = currentStage };
    }

    public override void LoadProgress(int stage)
    {
        currentStage = stage;

        if (doorAnimator != null)
        {
            if (currentStage == 1)
                doorAnimator.Play("DoorOpen", 0, 1f);
            else
                doorAnimator.Play("DoorClose", 0, 1f);
        }
        else
        {
            Debug.Log($"[Door] アニメーター未設定。currentStage={currentStage} のみ反映。");
        }

        isProcessing = false;
    }
}