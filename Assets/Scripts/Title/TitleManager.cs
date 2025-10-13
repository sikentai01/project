using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleMenuManager : MonoBehaviour
{
    [Header("�����I���{�^���ݒ�")]
    [SerializeField] private GameObject firstSelectedButton; // Start�{�^�����A�T�C��

    private void Start()
    {
        // �^�C�g���J�n���� START �{�^����I����Ԃɂ���
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    // --- �e�{�^���p���� ---

    /// <summary>
    /// �u���߂���v�{�^�����������Ƃ��� Scene0 �Ɉړ�
    /// </summary>
    public void OnStartGame()
    {
        // �V�[�������r���h�C���f�b�N�X�ǂ���ł�OK�i�����ł͖��O�Ŏw��j
        SceneManager.LoadScene("Scenes0");
    }

    /// <summary>
    /// �u�Â�����v�{�^���p�i�������ł�OK�j
    /// </summary>
    public void OnLoadGame()
    {
        Debug.Log("���[�h�@�\�͌�Œǉ��\��");
    }

    /// <summary>
    /// �u�I���v�{�^�����������Ƃ�
    /// </summary>
    public void OnExitGame()
    {
        Debug.Log("�Q�[���I��");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �G�f�B�^���Ȃ�Đ���~
#endif
    }
}
