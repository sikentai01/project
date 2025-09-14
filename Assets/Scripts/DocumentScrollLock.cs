using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DocumentScrollLock : ScrollRect
{
    // ホイール無効
    public override void OnScroll(PointerEventData data) { }

    // ドラッグ開始無効
    public override void OnBeginDrag(PointerEventData eventData) { }

    // ドラッグ中無効
    public override void OnDrag(PointerEventData eventData) { }

    // ドラッグ終了無効
    public override void OnEndDrag(PointerEventData eventData) { }
}