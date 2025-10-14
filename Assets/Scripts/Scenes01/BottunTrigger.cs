using UnityEngine;
using System.Collections.Generic; // List<T>を使うために必要

public class BottunTrigger : MonoBehaviour
{
    [Header("ギミック設定")]
    // Inspectorから、順番に表示したい子オブジェクトの親となるゲームオブジェクトを設定します。
    public GameObject KanzouGimmickParent;

    // Enterキーの連打を防ぐためのクールダウン時間
    public float inputDelay = 0.5f;

    [Header("内部情報")]
    // 現在表示されている子オブジェクトのインデックス（0から始まる）
    private int currentIndex = 0;

    // 子オブジェクトを格納するためのリスト
    private List<GameObject> gimmickChildren;

    // 次の入力が可能になる時刻
    private float nextInputTime = 0f;


    void Start()
    {
        // 1. リストの初期化
        gimmickChildren = new List<GameObject>();

        if (KanzouGimmickParent != null)
        {
            // 2. 親オブジェクトの子を全てリストに追加
            foreach (Transform child in KanzouGimmickParent.transform)
            {
                gimmickChildren.Add(child.gameObject);
                // 3. 最初は全て非表示にする
                child.gameObject.SetActive(false);
            }
        }

        Debug.Log("BottunTriggerが初期化されました。子オブジェクト数: " + gimmickChildren.Count);
    }

    void Update()
    {
        // 4. Enterキーの入力とクールダウンをチェック
        // ※ここでは、トリガーに接触しているかどうかの判定は省略しています。
        //   もしトリガー接触が必要なら、前回の説明を参考に実装してください。
        if (Input.GetKeyDown(KeyCode.Return) && Time.time > nextInputTime)
        {
            // 5. 子オブジェクトがまだ残っているか（この場合、インデックスが3以下か）を確認
            // 4個のオブジェクトはインデックス0, 1, 2, 3に対応します。
            if (currentIndex < gimmickChildren.Count)
            {
                // 6. 現在のインデックスの子オブジェクトを表示
                Debug.Log($"オブジェクトの表示: {gimmickChildren[currentIndex].name}");
                gimmickChildren[currentIndex].SetActive(true);

                // 7. 次のオブジェクトへインデックスを進める
                currentIndex++;

                // 8. 次の入力が可能になる時間を設定
                nextInputTime = Time.time + inputDelay;
            }
            else
            {
                // 4個全てを表示し終わった場合の処理
                Debug.Log("すべてのオブジェクト (4個) の表示が完了しました。");
                // 必要に応じて、ここで次の処理（例: シーンの進行、フラグのセットなど）を記述
            }
        }
    }
}