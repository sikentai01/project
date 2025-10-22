using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �Z�[�u�f�[�^�����[�h���ăQ�[����Ԃ����S��������B
/// �V�[�����E���W�E�����E�t���O�E�M�~�b�N�E�A�C�e���𐳊m�ɓK�p�B
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    public static SaveSystem.SaveData loadedData;

    private IEnumerator Start()
    {
        if (loadedData == null)
        {
            Debug.LogWarning("[GameBootstrap] loadedData ������܂���B�K�p�X�L�b�v�B");
            Destroy(gameObject);
            yield break;
        }

        // === �V�[���������Ƃɐ������V�[�������[�h ===
        string savedSceneName = loadedData.sceneName;
        if (!string.IsNullOrEmpty(savedSceneName))
        {
            Debug.Log($"[GameBootstrap] �Z�[�u���ꂽ�V�[�� '{savedSceneName}' ��ǂݍ��ݒ�...");

            var savedScene = SceneManager.GetSceneByName(savedSceneName);
            if (!savedScene.isLoaded)
            {
                var async = SceneManager.LoadSceneAsync(savedSceneName, LoadSceneMode.Additive);
                while (!async.isDone) yield return null;
                savedScene = SceneManager.GetSceneByName(savedSceneName);
                Debug.Log($"[GameBootstrap] {savedSceneName} �� Additive���[�h����");
            }

            // �V�[�����A�N�e�B�u��
            SceneManager.SetActiveScene(savedScene);
            yield return new WaitForEndOfFrame();
        }

        // ===== �C���x���g���E�h�L�������g =====
        if (InventoryManager.Instance != null && loadedData.inventoryData != null)
            InventoryManager.Instance.LoadData(loadedData.inventoryData);

        if (DocumentManager.Instance != null && loadedData.documentData != null)
            DocumentManager.Instance.LoadData(loadedData.documentData);

        // ===== �t���O =====
        if (GameFlags.Instance != null)
        {
            GameFlags.Instance.ClearAllFlags();

            if (loadedData.flagData != null && loadedData.flagData.activeFlags != null)
            {
                foreach (string flag in loadedData.flagData.activeFlags)
                    GameFlags.Instance.SetFlag(flag);

                Debug.Log($"[GameBootstrap] {loadedData.flagData.activeFlags.Length} ���̃t���O�𕜌����܂���");
            }

            // �Z�[�u�ɑ��݂��Ȃ��g���K�[��false����
            var allTriggers = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                .Where(x => x.gameObject.scene.isLoaded && x.GetType().Name.EndsWith("Trigger"));

            foreach (var obj in allTriggers)
            {
                string flagID = obj.GetType().Name;
                bool existsInSave = loadedData.flagData?.activeFlags?.Contains(flagID) ?? false;

                if (!existsInSave)
                {
                    GameFlags.Instance.RemoveFlag(flagID);
                    Debug.Log($"[GameBootstrap] {flagID} �̓Z�[�u�f�[�^�ɑ��݂��Ȃ����� false �����ɂ��܂���");
                }
            }
        }

        // ===== �v���C���[�ʒu�E���� =====
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            var playerMove = playerObj.GetComponent<GridMovement>();
            if (playerMove != null)
            {
                playerObj.transform.position = loadedData.playerPosition;
                playerMove.SetDirection(loadedData.playerDirection);
                Physics2D.SyncTransforms();
                Debug.Log($"[GameBootstrap] �v���C���[�ʒu�ƌ����𕜌�: {loadedData.playerPosition} / Dir={loadedData.playerDirection}");
            }
            else
            {
                Debug.LogWarning("[GameBootstrap] GridMovement �������炸�A�����̕������X�L�b�v���܂���");
            }
        }
        else
        {
            Debug.LogWarning("[GameBootstrap] �v���C���[�������炸�A���W�������X�L�b�v���܂���");
        }

        // ===== �M�~�b�N�i�s�x =====
        if (loadedData.gimmickProgressList != null && loadedData.gimmickProgressList.Count > 0)
        {
            var allGimmicks = Resources.FindObjectsOfTypeAll<GimmickBase>();
            foreach (var g in loadedData.gimmickProgressList)
            {
                var gimmick = allGimmicks.FirstOrDefault(x =>
                    x.gimmickID == g.gimmickID && x.gameObject.scene.isLoaded);

                if (gimmick != null)
                {
                    gimmick.LoadProgress(g.stage);
                    Debug.Log($"[GameBootstrap] �M�~�b�N {g.gimmickID} �� Stage={g.stage} �ɕ������܂���");
                }
            }
        }

        // ===== �A�C�e���g���K�[ =====
        if (loadedData.itemTriggerList != null && loadedData.itemTriggerList.Count > 0)
        {
            var allItemTriggers = Resources.FindObjectsOfTypeAll<ItemTrigger>();
            foreach (var i in loadedData.itemTriggerList)
            {
                var trigger = allItemTriggers.FirstOrDefault(x =>
                    x.triggerID == i.triggerID && x.gameObject.scene.isLoaded);

                if (trigger != null)
                {
                    trigger.LoadProgress(i.currentStage);
                    Debug.Log($"[GameBootstrap] �A�C�e���g���K�[ {i.triggerID} �� Stage={i.currentStage} �ɕ������܂���");
                }
            }
        }

        Debug.Log("[GameBootstrap] �Z�[�u�f�[�^�K�p�����i�V�[���{���W�{�����t�������j");

        loadedData = null;
        Destroy(gameObject);
    }
}