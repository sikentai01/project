using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // �^�C�g���V�[���ɖ߂�
            SceneManager.LoadScene("Title");
        }
    }
}