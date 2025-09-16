using UnityEngine;
using System.Collections;

public class GateTriggerController : MonoBehaviour
{
    // インスペクターから表示したいGateゲームオブジェクトを割り当てる
    public GameObject gateObject;

    // トリガーの中にいる間、毎フレーム呼び出される
    void OnTriggerStay2D(Collider2D other)
    {
        // プレイヤーに"Player"タグがついていることを確認
        if (other.CompareTag("Player"))
        {
            // Enterキーが押されたかチェック
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Gateゲームオブジェクトを有効化（表示）
                if (gateObject != null)
                {
                    gateObject.SetActive(true);
                }

                // 1秒後に非表示にするコルーチンを開始
                StartCoroutine(WaitAndHide());
            }
        }
    }

    private IEnumerator WaitAndHide()
    {
        // 1秒間待機
        yield return new WaitForSeconds(1.0f);

        // Gateゲームオブジェクトを無効化（非表示）
        if (gateObject != null)
        {
            gateObject.SetActive(false);
        }
    }
}