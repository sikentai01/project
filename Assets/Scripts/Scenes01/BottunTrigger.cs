using UnityEngine;
using System.Collections.Generic;

public class BottunTrigger : MonoBehaviour
{
    // 肝臓ギミックの親オブジェクト（Inspectorからドラッグ&ドロップで設定）
    public GameObject KanzouGimmickParent;

    // 現在表示されている子オブジェクトのインデックス
    private int currentIndex = 0;

    // 子オブジェクトのリスト（キャッシュ用）
    private List<GameObject> gimmickChildren;

    // 入力後のクールダウン時間（連打防止用）
    private float nextInputTime = 0f;
    public float inputDelay = 0.5f; // 例: 0.5秒の遅延
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 子オブジェクトのリストを初期化
        gimmickChildren = new List<GameObject>();

        if (KanzouGimmickParent != null)
        {
            // 親オブジェクトの子を全て取得
            foreach (Transform child in KanzouGimmickParent.transform)
            {
                gimmickChildren.Add(child.gameObject);
                // 最初は全て非表示にする
                child.gameObject.SetActive(false);
            }
        }
        currentIndex = 0; // インデックスをリセット
    }

    // Update is called once per frame
    void Update()
    {
        // ※BottunTriggerがプレイヤーに接触していることを示すフラグ（例: isPlayerInRange）が
        //   trueの時のみ、この処理を実行するようにするのが一般的です。

        // Enterキーが押されたか、または指定された入力（例: Input.GetKeyDown("return")）をチェック
        // および、クールダウン時間が経過したかをチェック
        if (Input.GetKeyDown(KeyCode.Return) && Time.time > nextInputTime)
        {
            // 子オブジェクトがまだ残っているかを確認
            if (currentIndex < gimmickChildren.Count)
            {
                // 現在のインデックスの子オブジェクトを表示
                gimmickChildren[currentIndex].SetActive(true);

                // 次のオブジェクトへインデックスを進める
                currentIndex++;

                // 次の入力が可能になる時間を設定
                nextInputTime = Time.time + inputDelay;
            }
            else
            {
                // すべて表示し終わった場合の処理（例: 何らかのメッセージ表示や次のフェーズへ移行）
                Debug.Log("肝臓ギミックの表示が完了しました。");
            }
        }
    }
}
