using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    public static SaveSystem.SaveData loadedData;

    void Start()
    {
        // --- ���[�h�f�[�^������ꍇ�̂ݓK�p ---
        if (loadedData != null)
        {
            Debug.Log("[GameBootstrap] ���[�h�f�[�^�K�p���J�n");

            // �v���C���[�̈ʒu�ƌ����𕜌�
            var player = FindFirstObjectByType<GridMovement>();
            if (player != null)
            {
                player.transform.position = loadedData.playerPosition;
                player.SetDirection(loadedData.playerDirection);
            }
            else
            {
                Debug.LogWarning("[GameBootstrap] �v���C���[��������܂���ł����B");
            }

            // --- �C���x���g������ ---
            if (InventoryManager.Instance != null && loadedData.inventoryData != null)
            {
                InventoryManager.Instance.LoadData(
                    new InventorySaveData
                    {
                        ownedItemIDs = new System.Collections.Generic.List<string>(
                            loadedData.inventoryData.ownedItemIDs
                        )
                    }
                );
            }

            // --- �������� ---
            if (DocumentManager.Instance != null && loadedData.documentData != null)
            {
                DocumentManager.Instance.LoadData(
                    new DocumentSaveData
                    {
                        obtainedIDs = new System.Collections.Generic.List<string>(
                            loadedData.documentData.obtainedIDs
                        )
                    }
                );
            }

            // --- �t���O���� ---
            if (GameFlags.Instance != null && loadedData.flagData != null)
            {
                GameFlags.Instance.LoadFlags(
                    new FlagSaveData
                    {
                        activeFlags = loadedData.flagData.activeFlags
                    }
                );
            }

            // --- Title�V�[���̃A�����[�h��x���Ŏ��s ---
            StartCoroutine(UnloadTitleAfterLoad());

            // --- �J�����m�F�iBootstrap�R�����ێ��j ---
            Camera cam = Camera.main;
            if (cam == null)
            {
                var newCam = new GameObject("MainCamera");
                cam = newCam.AddComponent<Camera>();
                cam.tag = "MainCamera";
                cam.transform.position = new Vector3(
                    loadedData.playerPosition.x,
                    loadedData.playerPosition.y,
                    -10f
                );
                newCam.AddComponent<AudioListener>();
                Debug.Log("[GameBootstrap] �J�������Ȃ��������ߐV�K�������܂����B");
            }
            else
            {
                if (cam.GetComponent<AudioListener>() == null)
                {
                    cam.gameObject.AddComponent<AudioListener>();
                    Debug.Log("[GameBootstrap] �����J������AudioListener��ǉ����܂����B");
                }
            }

            Debug.Log("[GameBootstrap] ���[�h�f�[�^�K�p�����B");
            loadedData = null; // ��x�g������j��
        }
    }

    // ==============================
    // Title�V�[�����m���ɃA�����[�h
    // ==============================
    private IEnumerator UnloadTitleAfterLoad()
    {
        // �����҂��Ȃ���Unity���V�[�������X�V���Ȃ�
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            string sceneName = s.name;

            // ������v�ł����o
            if (sceneName.Contains("Title"))
            {
                Debug.Log($"[GameBootstrap] Title�V�[��({sceneName})���A�����[�h���܂��B");
                yield return SceneManager.UnloadSceneAsync(s);
            }
        }

        // Additive���[�h�����Q�[���V�[�����A�N�e�B�u��
        Scene newScene = SceneManager.GetSceneByName("Scenes0");
        if (newScene.IsValid())
        {
            SceneManager.SetActiveScene(newScene);
            Debug.Log($"[GameBootstrap] �A�N�e�B�u�V�[���� {newScene.name} �ɕύX���܂����B");
        }
        else
        {
            Debug.LogWarning("[GameBootstrap] �Q�[���V�[��(Scenes0)��������܂���ł����B");
        }
    }
}