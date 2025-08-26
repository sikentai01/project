using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;  // ���̃|�[�Y���j���[
    public GameObject itemPanel;    // �E�̃A�C�e����� 

    public static bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        itemPanel.SetActive(false);    // �ŏ��̓A�C�e����ʂ���\��
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
        itemPanel.SetActive(false);   // ���j���[���鎞�Ɉꏏ�ɔ�\��
        Time.timeScale = 1f;
        isPaused = false;
        Debug.Log("�Q�[���ĊJ");
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
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
