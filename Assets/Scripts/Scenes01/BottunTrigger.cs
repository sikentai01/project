using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BottunTrigger : MonoBehaviour
{
    [Header("ギミック設定")]
    public GameObject KanzouGimmickParent;

    // オブジェクトの表示時間（表示し続ける時間）
    public float displayDuration = 0.5f;
    // 次のオブジェクトへ切り替わるまでの待機時間（非表示になっている時間）
    // ※今回は表示時間と同じ値を使用します。

    [Header("内部情報")]
    private List<GameObject> gimmickChildren;
    private bool isRunning = false;


    void Start()
    {
        gimmickChildren = new List<GameObject>();

        if (KanzouGimmickParent != null)
        {
            // 全ての子をリストに追加し、非表示にする
            foreach (Transform child in KanzouGimmickParent.transform)
            {
                gimmickChildren.Add(child.gameObject);
                child.gameObject.SetActive(false);
            }
        }

        Debug.Log("BottunTriggerが初期化されました。子オブジェクト数: " + gimmickChildren.Count);
    }

    void Update()
    {
        // Enterキーが押され、かつ現在処理が実行中でないことをチェック
        if (Input.GetKeyDown(KeyCode.Return) && !isRunning)
        {
            isRunning = true;
            StartCoroutine(DisplayObjectsSequentially());
        }
    }

    // コルーチン：オブジェクトを順番に表示→非表示にする処理
    private IEnumerator DisplayObjectsSequentially()
    {
        // 待機時間を設定
        WaitForSeconds waitDuration = new WaitForSeconds(displayDuration);

        // リスト内の各オブジェクトを順番に処理
        foreach (GameObject gimmick in gimmickChildren)
        {
            // 【1. 表示処理】
            Debug.Log($"オブジェクトを表示: {gimmick.name}");
            gimmick.SetActive(true);

            // 表示時間だけ待機
            yield return waitDuration;

            // 【2. 非表示処理】
            gimmick.SetActive(false);

            // 次のオブジェクトを表示するまでの待機時間（今回は同じ時間で設定）
            // 必要に応じて、ここで別の待機時間（yield return new WaitForSeconds(0.2f); など）を設けてください
        }

        // すべての処理が完了したらフラグを下ろす
        isRunning = false;
        Debug.Log("すべてのオブジェクトの表示と切り替えが完了しました。");
    }
}