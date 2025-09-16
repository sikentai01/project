using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ConfirmPanel : MonoBehaviour
{
    public static ConfirmPanel Instance;

    [Header("UI")]
    public GameObject panel;
    public Button yesButton;
    public Button noButton;
    public Text confirmText;   // "◯◯を使用しますか？" を表示する Text

    private ItemData currentItem;
    private GameObject currentSelected;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(ItemData item)
    {
        currentItem = item;
        panel.SetActive(true);

        // メッセージを更新
        confirmText.text = $"{item.itemName} を使用しますか？";

        // YES を最初に選択
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
        currentSelected = yesButton.gameObject;
    }

    public void Hide()
    {
        panel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        currentSelected = null;
    }

    void Update()
    {
        if (!panel.activeSelf) return;

        // マウス禁止
        if (Input.GetMouseButtonDown(0))
        {
            EventSystem.current.SetSelectedGameObject(currentSelected);
        }

        // 横矢印キーで YES/NO を切り替える
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentSelected == yesButton.gameObject)
            {
                EventSystem.current.SetSelectedGameObject(noButton.gameObject);
                currentSelected = noButton.gameObject;
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
                currentSelected = yesButton.gameObject;
            }
        }

        // Enterで決定
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentSelected == yesButton.gameObject)
            {
                UseItem();
            }
            else
            {
                Cancel();
            }
        }
    }

    private void UseItem()
    {
        Debug.Log($"{currentItem.itemName} を使用しました！");
        currentItem.Use();   // ScriptableObjectの Use() を呼ぶ
        Hide();
    }

    private void Cancel()
    {
        Debug.Log("アイテム使用をキャンセルしました");
        Hide();
    }
}