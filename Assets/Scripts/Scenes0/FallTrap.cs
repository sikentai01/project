using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FallTrap : MonoBehaviour
{
    private Collider2D trapCollider;

    void Start()
    {
        trapCollider = GetComponent<Collider2D>();

        // ���łɃt���O�������Ă���i��: �Z�[�u��j�Ȃ�㩂𖳌���
        if (GameFlags.Instance != null && GameFlags.Instance.HasFlag("SaveTriggered"))
        {
            DisableTrap();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �g���K�[�łȂ��ꍇ�͖���
        if (!trapCollider.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("[FallTrap] �v���C���[�����Ƃ����ɗ��� �� GameOver��");

            // �v���C���[�̑���𖳌���
            var move = other.GetComponent<GridMovement>();
            if (move != null)
            {
                move.enabled = false;
                Debug.Log("[FallTrap] �v���C���[������~���܂���");
            }

            // �����x�点�ăQ�[���I�[�o�[��ʂ�\��
            StartCoroutine(ShowGameOverRoutine());
        }
    }

    private IEnumerator ShowGameOverRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        // BootLoader���擾
        var boot = FindObjectOfType<BootLoader>();
        if (boot == null)
        {
            Debug.LogWarning("[FallTrap] BootLoader��������܂���BGameOver�𒼐ڗL�������܂��B");
        }

        // Scene�𒼐ڑ���iAdditive�\���ɑΉ��j
        Scene gameOverScene = SceneManager.GetSceneByName("GameOver");
        if (gameOverScene.IsValid() && gameOverScene.isLoaded)
        {
            // GameOver�V�[����ON
            foreach (var root in gameOverScene.GetRootGameObjects())
                root.SetActive(true);

            Debug.Log("[FallTrap] ������GameOver�V�[�����ėL�������܂���");
        }
        else
        {
            // �����[�h�Ȃ烍�[�h
            Debug.Log("[FallTrap] GameOver�V�[����V�K���[�h���܂�");
            yield return SceneManager.LoadSceneAsync("GameOver", LoadSceneMode.Additive);
        }

        // �Q�[���V�[���iScene�n�j���A�N�e�B�u��
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name.StartsWith("Scene"))
            {
                foreach (var root in s.GetRootGameObjects())
                    root.SetActive(false);
            }
        }

        // �m����GameOver���A�N�e�B�u�ɂȂ�悤�ݒ�
        Scene goScene = SceneManager.GetSceneByName("GameOver");
        if (goScene.IsValid())
            SceneManager.SetActiveScene(goScene);

        Debug.Log("[FallTrap] �Q�[���V�[����\�� �� GameOver�\������");
    }

    // ====== ���Ƃ������u�������v ======
    public void DisableTrap()
    {
        if (trapCollider != null)
        {
            trapCollider.isTrigger = false; // �ǈ����ɂ���
            Debug.Log("[FallTrap] ���Ƃ����𖳌��� �� �ǂɂ��܂���");
        }
    }
}