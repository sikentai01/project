using UnityEngine;
using TMPro;

public class DocumentSlot : MonoBehaviour
{
    private DocumentManager.DocumentData currentDoc;
    public TMP_Text titleText;   // �X���b�g�ɕ\������^�C�g��

    /// <summary>
    /// �������Z�b�g
    /// </summary>
    public void SetDocument(DocumentManager.DocumentData doc)
    {
        currentDoc = doc;
        titleText.text = doc.title;
    }

    /// <summary>
    /// �X���b�g����ɂ���
    /// </summary>
    public void ClearSlot()
    {
        currentDoc = null;
        titleText.text = "";
    }

    /// <summary>
    /// �X���b�g���N���b�N�����Ƃ�
    /// </summary>
    public void OnClickSlot()
    {
        if (currentDoc != null)
        {
            int index = System.Array.IndexOf(DocumentManager.Instance.documents, currentDoc);
            if (index >= 0)
            {
                DocumentManager.Instance.ShowDocument(index);
            }
        }
    }
}