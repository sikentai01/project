using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DocumentScrollLock : ScrollRect
{
    // �z�C�[������
    public override void OnScroll(PointerEventData data) { }

    // �h���b�O�J�n����
    public override void OnBeginDrag(PointerEventData eventData) { }

    // �h���b�O������
    public override void OnDrag(PointerEventData eventData) { }

    // �h���b�O�I������
    public override void OnEndDrag(PointerEventData eventData) { }
}