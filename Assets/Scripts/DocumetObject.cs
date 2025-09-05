using UnityEngine;

public class DocumentObject : MonoBehaviour
{
    [Header("�������")]
    public int documentID;      // �����̌ŗLID�i�Z�[�u�p �� int�^�ɏC���j
    public string title;        // �����^�C�g��

    private bool isPlayerNear = false;

    void Update()
    {
        // �v���C���[���߂��� Enter �������ꂽ�����
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
            Debug.Log($"�����u{title}�v����肵�܂����I");

            // ��x���肵��������i�o���Ȃ����邾���ł��j
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