using UnityEngine;
using UnityEngine.EventSystems;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton; // ← Start ボタンをアサイン

    void Start()
    {
        // シーン開始時に START ボタンを選択状態にする
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}
