using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleMenuManager : MonoBehaviour
{
    [Header("�����I���{�^���ݒ�")]
    [SerializeField] private GameObject firstSelectedButton; // Start�{�^�����A�T�C��

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    // --- �e�{�^���p���� ---

    /// <summary>
    /// �u���߂���v�{�^�����������Ƃ��� Scene0 �Ɉړ�
    /// </summary>
    public void OnStartGame()
    {
        Debug.Log("�Q�[���J�n");

        // Additive���[�h�iBootstrap��ێ��j
        RoomLoader.LoadRoom("Scenes0",null);

        // Title���ォ��j��
        StartCoroutine(UnloadTitleScene());
    }

    private IEnumerator UnloadTitleScene()
    {
        yield return new WaitForSeconds(0.5f);
        Scene current = SceneManager.GetSceneByName("Title");
        if (current.IsValid() && current.isLoaded)
        {
            SceneManager.UnloadSceneAsync(current);
            Debug.Log("Title�V�[����j�����܂���");
        }
    }

    public void OnLoadGame()
    {
        Debug.Log("���[�h�@�\�͌�Œǉ��\��");
    }

    public void OnExitGame()
    {
        Debug.Log("�Q�[���I��");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
