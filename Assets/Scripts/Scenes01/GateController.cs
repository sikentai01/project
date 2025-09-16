using UnityEngine;

public class GateController : MonoBehaviour
{
    // インスペクターから表示したいGateゲームオブジェクトを割り当てる
    public GameObject gateObject;

    void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーがトリガーに入ったことを確認
        if (other.CompareTag("Player"))
        {
            // プレイヤーがトリガー内にいる間、Enterキーの入力をチェック
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // Gateゲームオブジェクトを有効化
                if (gateObject != null)
                {
                    gateObject.SetActive(true);
                }
            }
        }
    }
}