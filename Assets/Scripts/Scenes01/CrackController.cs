using UnityEngine;
using System.Collections;

public class CrackController : MonoBehaviour
{
    public GameObject crackEffect;
    // playerMovementScriptをインスペクターから割り当てる代わりに、スクリプト内で自動で検索する
    private MonoBehaviour playerMovementScript;

    void Start()
    {
        // シーンに関係なくGridMovementスクリプトを検索して割り当てる
        playerMovementScript = FindObjectOfType<GridMovement>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (crackEffect != null)
            {
                crackEffect.SetActive(true);
            }

            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = false;
            }

            StartCoroutine(WaitAndResume());
            gameObject.SetActive(false);
        }
    }

    private IEnumerator WaitAndResume()
    {
        yield return new WaitForSeconds(1.0f);

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }
}