using UnityEngine;
using TMPro;

public class DocumentSlot : MonoBehaviour
{
    [Header("サウンド設定")]
    [SerializeField] private AudioClip clickSeClip;
    private DocumentManager.DocumentData currentDoc;
    public TMP_Text titleText;   // スロットに表示するタイトル

    /// <summary>
    /// 資料をセット
    /// </summary>
    public void SetDocument(DocumentManager.DocumentData doc)
    {
        currentDoc = doc;
        titleText.text = doc.title;
    }

    /// <summary>
    /// スロットを空にする
    /// </summary>
    public void ClearSlot()
    {
        currentDoc = null;
        titleText.text = "";
    }

    /// <summary>
    /// スロットをクリックしたとき
    /// </summary>
    public void OnClickSlot()
    {
        if (currentDoc != null)
        {
            if (SoundManager.Instance != null && clickSeClip != null)
            {
                SoundManager.Instance.PlaySE(clickSeClip);
            }
            int index = System.Array.IndexOf(DocumentManager.Instance.documents, currentDoc);
            if (index >= 0)
            {
                DocumentManager.Instance.ShowDocument(index);
            }
        }
    }
}