using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public static bool isGameOver = false; // �����ǉ��F�Q�[���I�[�o�[�t���O

    [SerializeField] private AudioClip confirmSeClip;

    void OnEnable()
    {
        isGameOver = true;
        Time.timeScale = 0f; // �����Q�[�����̓������~�߂�
    }

    void OnDisable()
    {
        isGameOver = false;
        Time.timeScale = 1f; // �����ĊJ���߂�
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("[GameOverController] �^�C�g���ɖ߂�");

            if (SoundManager.Instance != null && confirmSeClip != null)
                SoundManager.Instance.PlaySE(confirmSeClip);

            ReturnToTitle();
        }
    }

    private void ReturnToTitle()
    {
        // �Q�[���V�[����S�Ĕ�A�N�e�B�u��
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name.StartsWith("Scene"))
            {
                foreach (var root in s.GetRootGameObjects())
                    root.SetActive(false);
            }
        }

        // GameOver UI���\��
        Scene current = SceneManager.GetSceneByName("GameOver");
        if (current.isLoaded)
        {
            foreach (var root in current.GetRootGameObjects())
                root.SetActive(false);
        }

        // Title���ėL����
        Scene titleScene = SceneManager.GetSceneByName("Title");
        if (titleScene.isLoaded)
        {
            foreach (var root in titleScene.GetRootGameObjects())
                root.SetActive(true);
        }
        else
        {
            SceneManager.LoadSceneAsync("Title", LoadSceneMode.Additive);
        }

        Debug.Log("[GameOverController] �^�C�g���ւ̕��A����");
    }
}