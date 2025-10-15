using UnityEngine;
using UnityEngine.SceneManagement;

public class FallTrap : MonoBehaviour
{
    private Collider2D trapCollider;

    void Start()
    {
        trapCollider = GetComponent<Collider2D>();

        // �����Z�[�u�ς݂Ȃ�A�ŏ�����g���K�[OFF�ɂ��ĕǉ�
        if (GameFlags.SaveTriggered)
        {
            DisableTrap();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �g���K�[����Ȃ����͉������Ȃ�
        if (!trapCollider.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log("�v���C���[�����Ƃ����ɗ������I");

            //  RoomLoader���g����GameOver��Additive�Ń��[�h
            RoomLoader.LoadRoom("GameOver", null);

            // ���݂̃V�[�����A�����[�h�iAdditive�\���̐����j
            StartCoroutine(UnloadCurrentScene());
        }
    }

    private System.Collections.IEnumerator UnloadCurrentScene()
    {
        yield return new WaitForSeconds(0.5f);

        Scene current = SceneManager.GetActiveScene();
        if (current.IsValid() && current.isLoaded && current.name.StartsWith("Scene"))
        {
            SceneManager.UnloadSceneAsync(current);
            Debug.Log($"{current.name} ��j�����܂���");
        }
    }

    // ====== �����ŗ��Ƃ������u�������v ======
    public void DisableTrap()
    {
        if (trapCollider != null)
        {
            trapCollider.isTrigger = false; // �� ����ŕǂɂ���
            Debug.Log("���Ƃ����𖳌��� �� �ǂɂ��܂���");
        }
    }
}
