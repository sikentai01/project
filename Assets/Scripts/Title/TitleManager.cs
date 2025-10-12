using UnityEngine;
using UnityEngine.EventSystems;

public class TitleMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton; // �� Start �{�^�����A�T�C��

    void Start()
    {
        // �V�[���J�n���� START �{�^����I����Ԃɂ���
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}
