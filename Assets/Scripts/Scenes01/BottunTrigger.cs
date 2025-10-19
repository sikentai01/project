using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BottunTrigger : MonoBehaviour
{
    [Header("ギミック設定")]
    public GameObject KanzouGimmickParent;

    // オブジェクトの表示時間（表示し続ける時間）
    public float displayDuration = 0.5f;

    [Header("内部情報")]
    private List<GameObject> gimmickChildren;
    private bool isRunning = false; // ギミックが動作中かどうか

    [Header("接触設定")]
    private bool isPlayerInRange = false;
    private const string PlayerTag = "Player"; // プレイヤーのタグ名


    // -----------------------------------------------------
    // 初期化処理
    // -----------------------------------------------------

    void Start()
    {
        gimmickChildren = new List<GameObject>();

        if (KanzouGimmickParent != null)
        {
            // 全ての子をリストに追加し、最初は非表示にする
            foreach (Transform child in KanzouGimmickParent.transform)
            {
                gimmickChildren.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }

        // ★注意: この状態では、playerControllerの参照・取得処理は含まれていません。★

        Debug.Log("BottunTriggerが初期化されました。子オブジェクト数: " + gimmickChildren.Count);
    }

    // -----------------------------------------------------
    // メイン更新処理
    // -----------------------------------------------------

    void Update()
    {
        // 1. 範囲外、またはギミック動作中は処理を終了
        if (!isPlayerInRange || isRunning)
        {
            return;
        }

        // 2. Enterキーが押されたかチェック
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isRunning = true;

            // ★プレイヤー停止処理はここには含まれません★

            // ギミック動作開始
            StartCoroutine(DisplayObjectsSequentially());
        }
    }

    // -----------------------------------------------------
    // ギミック処理 (コルーチン)
    // -----------------------------------------------------

    private IEnumerator DisplayObjectsSequentially()
    {
        WaitForSeconds waitDuration = new WaitForSeconds(displayDuration);

        // リスト内の各オブジェクトを順番に表示・非表示
        foreach (GameObject gimmick in gimmickChildren)
        {
            // 1. 表示処理
            Debug.Log($"オブジェクトを表示: {gimmick.name}");
            gimmick.SetActive(true);

            // 表示時間だけ待機
            yield return waitDuration;

            // 2. 非表示処理
            gimmick.SetActive(false);
        }

        // ギミック処理が完了
        isRunning = false;

        // ★プレイヤー復帰処理はここには含まれません★

        Debug.Log("すべてのオブジェクトの表示と切り替えが完了しました。");
    }

    // -----------------------------------------------------
    // 接触判定
    // -----------------------------------------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            isPlayerInRange = true;
            Debug.Log("プレイヤーが範囲内に入りました。");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(PlayerTag))
        {
            isPlayerInRange = false;
            Debug.Log("プレイヤーが範囲外に出ました。");
        }
    }
}