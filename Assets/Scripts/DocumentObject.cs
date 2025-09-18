using UnityEngine;

public class DocumentObject : MonoBehaviour
{
    [Header("資料情報")]
    public string documentID;  // ← int → string に
    public string title;       // 資料タイトル

    private bool isPlayerNear = false;

    void Update()
    {
        // プレイヤーが近くにいて、Enterキーが押されたら入手
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            CollectDocument();
        }
    }

    public void CollectDocument()
    {
        if (DocumentManager.Instance != null)
        {
            DocumentManager.Instance.AddDocument(documentID, title);
            Debug.Log($"資料「{title}」を入手しました");
        }

        gameObject.SetActive(false); // 入手後に消す（非表示でもOK）
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