using UnityEngine;

public class DocumentObject : MonoBehaviour
{
    [Header("�������")]
    public int documentID;    // ����ID�i�Z�[�u�p�j
    public string title;      // �����^�C�g��

    private bool isPlayerNear = false;

    void Update()
    {
        // �v���C���[���߂��ɂ��āAEnter�L�[�������ꂽ�����
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            CollectDocument();
        }
    }

    void CollectDocument()
    {
        // DocumentManager �ɓo�^
        if (DocumentManager.Instance != null)
        {
            DocumentManager.Instance.AddDocument(documentID, title);
            Debug.Log($"�����u{title}�v����肵�܂���");
        }

        // ��x���肵���������i�����Ȃ����邾���ł�OK�j
        Destroy(gameObject);
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