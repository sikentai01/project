using UnityEngine;

public class CrackController : MonoBehaviour
{
    // インスペクターからヒビのゲームオブジェクトを割り当てる
    public GameObject crackEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 接触したオブジェクトがYuki（プレイヤー）であることを確認
        if (other.CompareTag("Player"))
        {
            // ヒビのゲームオブジェクトを有効化して表示
            if (crackEffect != null)
            {
                crackEffect.SetActive(true);
            }

            // このトリガーは一度しか使わないため、無効化する
            gameObject.SetActive(false);
        }
    }
}