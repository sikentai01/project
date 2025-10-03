using UnityEngine;

public class ButtonTriggerController : MonoBehaviour
{
    // インスペクターから肝臓ギミックの親オブジェクトを割り当てる
    public Transform liverGimmickParent;

    // ギミックの子オブジェクトのリスト（今回は自動で取得）
    private GameObject[] gimmickChildren;

    // プレイヤーがトリガー内にいるかどうか
    private bool isPlayerNear = false;

    void Start()
    {
        if (liverGimmickParent == null)
        {
            Debug.LogError("肝臓ギミックの親オブジェクトが割り当てられていません！");
            return;
        }

        // 肝臓ギミックの子オブジェクトを配列に格納
        int childCount = liverGimmickParent.childCount;
        gimmickChildren = new GameObject[childCount];

        for (int i = 0; i < childCount; i++)
        {
            gimmickChildren[i] = liverGimmickParent.GetChild(i).gameObject;
            // 全ての子オブジェクトを非表示にしておく
            gimmickChildren[i].SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            // 静的クラスから現在のインデックスを取得
            int nextIndex = GimmickState.LiverGimmickIndex;

            if (nextIndex < gimmickChildren.Length)
            {
                // 該当の子オブジェクトを表示
                gimmickChildren[nextIndex].SetActive(true);

                // 次のギミックのためにインデックスを更新
                GimmickState.LiverGimmickIndex++;

                Debug.Log($"ギミック {nextIndex + 1} を表示しました。");
            }
            else
            {
                Debug.Log("全てのギミックが完了しました。");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}