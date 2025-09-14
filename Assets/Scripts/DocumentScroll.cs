using UnityEngine;
using UnityEngine.UI;

public class DocumentScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 0.2f;

    void Update()
    {
        if (!scrollRect.gameObject.activeInHierarchy) return;

        float move = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) move = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) move = -1f;

        if (move != 0f)
        {
            float newPos = Mathf.Clamp01(scrollRect.verticalNormalizedPosition + move * scrollSpeed * Time.deltaTime * 50);
            scrollRect.verticalNormalizedPosition = newPos;
        }
    }
}
