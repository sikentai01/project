using UnityEngine;

public class DocumentObject : MonoBehaviour
{
    [Header("資料情報")]
    public int documentID;      // 資料の固有ID（セーブ用 → int型に修正）
    public string title;        // 資料タイトル

    private bool isPlayerNear = false;

    void Update()
    {
        // プレイヤーが近くて Enter が押されたら入手
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            CollectDocument();
        }
    }

    void CollectDocument()
    {
        if (DocumentManager.Instance != null)
        {
            DocumentManager.Instance.AddDocument(documentID, title);
            Debug.Log($"資料「{title}」を入手しました！");

            // 一度入手したら消す（覚えなくするだけでも可）
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }
}