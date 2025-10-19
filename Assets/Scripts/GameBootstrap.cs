using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrap : MonoBehaviour
{
    public static SaveSystem.SaveData loadedData;

    private void Start()
    {
        if (loadedData == null)
        {
            Debug.Log("[GameBootstrap] ���[�h�f�[�^�����݂��܂���B�ʏ�N���𑱍s�B");
            return;
        }

        Debug.Log("[GameBootstrap] ���[�h�f�[�^�K�p�J�n");

        // BootLoader���擾
        var boot = FindFirstObjectByType<BootLoader>();
        if (boot == null)
        {
            Debug.LogError("[GameBootstrap] BootLoader��������܂���I");
            return;
        }

        // Title���\���ɂ��A���[�h�ΏۃV�[����L����
        foreach (var kv in boot.loadedScenes)
            boot.SetSceneActive(kv.Key, kv.Key == loadedData.sceneName);

        // �A�N�e�B�u�V�[���ύX
        var targetScene = SceneManager.GetSceneByName(loadedData.sceneName);
        if (targetScene.IsValid())
            SceneManager.SetActiveScene(targetScene);

        // === �f�[�^���f ===
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = loadedData.playerPosition;
            var move = player.GetComponent<GridMovement>();
            if (move != null)
            {
                move.SetDirection(loadedData.playerDirection);
                move.enabled = true;
            }
        }

        if (InventoryManager.Instance != null && loadedData.inventoryData != null)
            InventoryManager.Instance.LoadData(loadedData.inventoryData);
        if (DocumentManager.Instance != null && loadedData.documentData != null)
            DocumentManager.Instance.LoadData(loadedData.documentData);
        if (GameFlags.Instance != null && loadedData.flagData != null)
            GameFlags.Instance.LoadFlags(loadedData.flagData);

        TitleManager.isTitleActive = false;
        PauseMenu.blockMenu = false;
        GameOverController.isGameOver = false;

        Debug.Log($"[GameBootstrap] {loadedData.sceneName} �̏�Ԃ𕜌����܂����B");

        loadedData = null;
    }
}