using UnityEngine;

public class CrackController : MonoBehaviour
{
    public GameObject crackEffect;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (crackEffect != null)
            {
                // ヒビのゲームオブジェクトを有効化
                crackEffect.SetActive(true);
            }

            // このトリガーは一度しか使わないため、無効化
            gameObject.SetActive(false);
        }
    }
}