using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // タイトルシーンに戻る
            SceneManager.LoadScene("Title");
        }
    }
}