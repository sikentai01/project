using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;  // UI�I���𑀍삷�邽�߂ɕK�v

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;   // ���̃|�[�Y���j���[
    public GameObject itemPanel;     // �E�̃A�C�e�����
    public GameObject firstSelected; // �ŏ��ɑI�������{�^���iItems�Ȃǁj

    public static bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        itemPanel.SetActive(false);   // �ŏ��͔�\��
        Time.timeScale = 1f;
        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        itemPanel.SetActive(false);   // �A�C�e����ʂ��ꏏ�ɕ���
        Time.timeScale = 1f;
        isPaused = false;

        // �I�������i�Q�[���ĊJ���̓J�[�\���������j
        EventSystem.current.SetSelectedGameObject(null);

        Debug.Log("�Q�[���ĊJ");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        //  �ŏ��Ɂu�A�C�e���v�{�^���ɃJ�[�\�������킹��
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        Debug.Log("�|�[�Y���j���[���J����");
    }

    // �� �A�C�e����ʂ��J��
    public void OpenItems()
    {
        itemPanel.SetActive(true);
        Debug.Log("�A�C�e����ʂ��J����");
    }

    // �� �A�C�e����ʂ����
    public void CloseItems()
    {
        itemPanel.SetActive(false);
    }

    public void OpenDocuments()
    {
        Debug.Log("������ʂ��J���i�_�~�[�j");
    }

    public void OpenOptions()
    {
        Debug.Log("�I�v�V������ʂ��J���i�_�~�[�j");
    }

    public void LoadGame()
    {
        Debug.Log("���[�h�����i�_�~�[ or Scene�ؑւ���Œǉ��j");
        // SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Debug.Log("�Q�[���I��");
        Application.Quit();
    }
}
