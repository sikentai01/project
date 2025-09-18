using UnityEngine;

public class DocumentObject : MonoBehaviour
{
    [Header("�������")]
    public string documentID;  // �� int �� string ��
    public string title;       // �����^�C�g��

    private bool isPlayerNear = false;

    void Update()
    {
        // �v���C���[���߂��ɂ��āAEnter�L�[�������ꂽ�����
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
            Debug.Log($"�����u{title}�v����肵�܂���");
        }

        gameObject.SetActive(false); // �����ɏ����i��\���ł�OK�j
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