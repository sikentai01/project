using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ScrollViewFocus : MonoBehaviour
{
    public ScrollRect scrollRect; // ScrollView を割り当て
    public RectTransform content; // Content を割り当て
    private RectTransform lastTarget;
    private Coroutine scrollCoroutine;

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        if (selected.transform.IsChildOf(content))
        {
            RectTransform target = selected.GetComponent<RectTransform>();
            if (target != null && target != lastTarget)
            {
                EnsureVisible(target);
                lastTarget = target;
            }
        }
    }

    void EnsureVisible(RectTransform target)
    {
        RectTransform viewport = scrollRect.viewport;

        // ターゲットの座標をビューポート座標系に変換
        Vector3 localPos = viewport.InverseTransformPoint(target.position);

        float viewportHeight = viewport.rect.height;
        float itemHeight = target.rect.height;

        // 下に見切れている
        if (localPos.y < -viewportHeight / 2f + itemHeight)
        {
            float diff = (-viewportHeight / 2f + itemHeight) - localPos.y;
            Vector3 newPos = content.localPosition + new Vector3(0, diff, 0);
            StartSmoothScroll(newPos);
        }
        // 上に見切れている
        else if (localPos.y > viewportHeight / 2f)
        {
            float diff = localPos.y - viewportHeight / 2f;
            Vector3 newPos = content.localPosition - new Vector3(0, diff, 0);
            StartSmoothScroll(newPos);
        }
    }


    void StartSmoothScroll(Vector3 targetPos)
    {
        if (scrollCoroutine != null)
        {
            StopCoroutine(scrollCoroutine);
        }
        scrollCoroutine = StartCoroutine(SmoothScroll(targetPos));
    }

    IEnumerator SmoothScroll(Vector3 targetPos)
    {
        Vector3 startPos = content.localPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 10f; // 数字を大きくすると速くスクロールする
            content.localPosition = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        content.localPosition = targetPos; // 最終位置を補正
    }
}
