using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // �� �ǉ��iUI����ɕK�v�j

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;   // Panel���A�^�b�`
    public GameObject firstButton;   // �ŏ��ɑI�������{�^�� (Items���A�^�b�`)
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false);  // �ŏ��͕K����\���ɂ���
        Time.timeScale = 1f;           // �Q�[���X�s�[�h�����Z�b�g
        isPaused = false;              // �t���O�����Z�b�g
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

        // �� ���j���[���J�����Ƃ��� Items �{�^���ɃJ�[�\�������킹��
        EventSystem.current.SetSelectedGameObject(null);        // ��x���Z�b�g
        EventSystem.current.SetSelectedGameObject(firstButton); // Items��I��
    }

    // �� �_�~�[�{�^������ ��
    public void OpenItems()
    {
        Debug.Log("�A�C�e����ʂ��J���i�_�~�[�j");
    }

    public void OpenDocuments()
    {
        Debug.Log("������ʂ��J���i�_�~�[�j");
    }

    public void OpenOptions()
    {
        Debug.Log("�I�v�V������ʂ��J���i�_�~�[�j");
    }

    // �� ���ۂɏ�������{�^�� ��
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
