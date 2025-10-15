using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverController : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("�^�C�g���ɖ߂�");
            RoomLoader.LoadRoom("Title", null);
            StartCoroutine(UnloadGameOver());
        }
    }

    private IEnumerator UnloadGameOver()
    {
        yield return new WaitForSeconds(0.5f);
        Scene current = SceneManager.GetSceneByName("GameOver");
        if (current.IsValid() && current.isLoaded)
        {
            SceneManager.UnloadSceneAsync(current);
            Debug.Log("GameOver�V�[����j�����܂���");
        }
    }
}
