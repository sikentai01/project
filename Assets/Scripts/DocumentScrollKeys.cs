using UnityEngine;
using UnityEngine.UI;

public class DocumentScrollKeys : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 1f;

    private bool skipFirstInput = false;

    void Update()
    {
        if (!scrollRect) return;

        float move = 0f;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            // 押された瞬間はスキップ
            skipFirstInput = true;
        }

        if (Input.GetKey(KeyCode.UpArrow)) move = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) move = -1f;

        if (move != 0f)
        {
            if (skipFirstInput)
            {
                skipFirstInput = false;
                return; // 最初の1フレーム目は処理しない
            }

            float dt = Mathf.Min(Time.unscaledDeltaTime, 0.005f);

            float newPos = Mathf.Clamp01(
                scrollRect.verticalNormalizedPosition + move * scrollSpeed * dt
            );
            scrollRect.verticalNormalizedPosition = newPos;

            Debug.Log($"Move:{move}, dt:{dt}, Pos:{scrollRect.verticalNormalizedPosition}");
        }
    }
}