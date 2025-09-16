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
    public Text confirmText;   // "�������g�p���܂����H" ��\������ Text

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

        // ���b�Z�[�W���X�V
        confirmText.text = $"{item.itemName} ���g�p���܂����H";

        // YES ���ŏ��ɑI��
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

        // �}�E�X�֎~
        if (Input.GetMouseButtonDown(0))
        {
            EventSystem.current.SetSelectedGameObject(currentSelected);
        }

        // �����L�[�� YES/NO ��؂�ւ���
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

        // Enter�Ō���
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
        Debug.Log($"{currentItem.itemName} ���g�p���܂����I");
        currentItem.Use();   // ScriptableObject�� Use() ���Ă�
        Hide();
    }

    private void Cancel()
    {
        Debug.Log("�A�C�e���g�p���L�����Z�����܂���");
        Hide();
    }
}