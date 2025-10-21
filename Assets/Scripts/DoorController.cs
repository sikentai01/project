using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 共通ドアコントローラー（シーン切替/同一シーン内対応）
/// </summary>
public class DoorController : GimmickBase
{
    public enum DoorMoveType
    {
        SameScene,
        ChangeScene
    }

    [Header("移動設定")]
    [SerializeField] private DoorMoveType moveType = DoorMoveType.SameScene;

    [Header("シーン切替用（ChangeScene時のみ）")]
    [SerializeField] private string targetScene = "";
    [SerializeField] private string targetPointName = "SpawnPoint";

    [Header("同一シーン内（SameScene時のみ）")]
    [SerializeField] private Transform targetPoint;

    [Header("鍵設定")]
    [SerializeField] private string requiredKeyID = "";
    [SerializeField] private string unlockFlag = "";

    [Header("開ける方向 (0=下,1=左,2=右,3=上)")]
    [SerializeField] private int requiredDirection = 0;

    [Header("演出")]
    [SerializeField] private Animator doorAnimator;
    [SerializeField] private AudioClip openSE;
    [SerializeField] private AudioClip lockedSE;
    [SerializeField] private AudioClip unlockSE;

    [Header("システムテキスト（仮）")]
    [SerializeField] private string systemTextWhenLocked = "鍵がかかっている…";
    [SerializeField] private string systemTextWhenUnlocked = "{0}で鍵を開けた";

    private Transform player;
    private GridMovement playerMove;
    private bool isPlayerNear = false;
    private bool isProcessing = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = true;
        player = other.transform;
        playerMove = player.GetComponent<GridMovement>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        isPlayerNear = false;
        player = null;
        playerMove = null;
    }

    private void Update()
    {
        if (!isPlayerNear || player == null || isProcessing) return;

        if (CheckInputDirection())
        {
            if (currentStage == 0)
                TryUnlock();
            else
                StartCoroutine(OpenDoorAndMove());
        }
    }

    private bool CheckInputDirection()
    {
        bool pressed = false;
        int dir = -1;

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) { dir = 0; pressed = true; }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) { dir = 1; pressed = true; }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) { dir = 2; pressed = true; }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) { dir = 3; pressed = true; }

        if (!pressed || playerMove == null) return false;
        return (playerMove.GetDirection() == requiredDirection && dir == requiredDirection);
    }

    private void TryUnlock()
    {
        if (!string.IsNullOrEmpty(requiredKeyID))
        {
            if (InventoryManager.Instance.HasItem(requiredKeyID))
            {
                InventoryManager.Instance.RemoveItemByID(requiredKeyID);
                currentStage = 1;
                GameFlags.Instance?.SetFlag(unlockFlag);

                string itemName = InventoryManager.Instance.allItems
                    .Find(i => i.itemID == requiredKeyID)?.itemName ?? "鍵";

                Debug.Log($"[Door] {string.Format(systemTextWhenUnlocked, itemName)}");
                SoundManager.Instance?.PlaySE(unlockSE);
                SoundManager.Instance?.PlaySE(openSE);
                StartCoroutine(OpenDoorAndMove());
            }
            else
            {
                Debug.Log($"[Door] {systemTextWhenLocked}");
                SoundManager.Instance?.PlaySE(lockedSE);
                doorAnimator?.SetTrigger("Locked");
            }
        }
        else
        {
            currentStage = 1;
            StartCoroutine(OpenDoorAndMove());
        }
    }

    private IEnumerator OpenDoorAndMove()
    {
        isProcessing = true;

        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("Open");
            yield return new WaitForSeconds(0.6f);
        }
        else
        {
            yield return new WaitForSeconds(0.6f);
        }

        SoundManager.Instance?.PlaySE(openSE);

        if (moveType == DoorMoveType.SameScene)
        {
            if (targetPoint != null && player != null)
            {
                player.position = targetPoint.position;
                Physics2D.SyncTransforms();
                Debug.Log($"[Door] 同一シーンで {targetPoint.name} に移動");
            }
            else
            {
                Debug.LogWarning("[Door] targetPoint 未指定 or Player 不在");
            }
        }
        else
        {
            yield return StartCoroutine(TransitionToScene());
        }

        isProcessing = false;
    }

    private IEnumerator TransitionToScene()
    {
        Debug.Log($"[DoorController] BootLoader経由でシーン遷移開始: {targetScene}");

        if (BootLoader.Instance != null)
        {
            BootLoader.Instance.RequestSceneSwitch(targetScene, targetPointName);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            Debug.LogWarning("[DoorController] BootLoaderなし。直接移動します。");
            var spawn = GameObject.Find(targetPointName);
            if (spawn != null && player != null)
                player.position = spawn.transform.position;
        }

        Physics2D.SyncTransforms();
    }

    public override GimmickSaveData SaveProgress()
    {
        return new GimmickSaveData
        {
            gimmickID = this.gimmickID,
            stage = this.currentStage
        };
    }

    public override void LoadProgress(int stage)
    {
        this.currentStage = stage;
    }
}