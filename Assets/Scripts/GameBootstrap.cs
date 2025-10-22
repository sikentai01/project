using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���[�h����ɃZ�[�u�f�[�^���Q�[���֔��f����N���X�B
/// SaveSlotButton ���琶������A�K�p��Ɏ��󂵂܂��B
/// Additive���[�h�\���ɂ��Ή��B
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    public static SaveSystem.SaveData loadedData; // SaveSlotButton �����������

    private IEnumerator Start()
    {
        if (loadedData == null)
        {
            Debug.LogWarning("[GameBootstrap] loadedData ������܂���B�����K�p���܂���B");
            Destroy(gameObject);
            yield break;
        }

        // 1�t���[���҂���Additive���[�h�̊�����҂�
        yield return null;

        // ===== �C���x���g���E�h�L�������g�E�t���O�K�p =====
        if (InventoryManager.Instance != null && loadedData.inventoryData != null)
            InventoryManager.Instance.LoadData(loadedData.inventoryData);

        if (DocumentManager.Instance != null && loadedData.documentData != null)
            DocumentManager.Instance.LoadData(loadedData.documentData);

        if (GameFlags.Instance != null && loadedData.flagData != null)
            GameFlags.Instance.LoadFlags(loadedData.flagData);

        // ===== �v���C���[�ʒu�E���� =====
        var player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<GridMovement>();
        if (player != null)
        {
            player.transform.position = loadedData.playerPosition;
            player.SetDirection(loadedData.playerDirection);
        }
        else
        {
            Debug.LogWarning("[GameBootstrap] �v���C���[��������܂���i�ʒu�E�����K�p�X�L�b�v�j");
        }

        // ===== �M�~�b�N�i�s�x�̕��� =====
        if (loadedData.gimmickProgressList != null && loadedData.gimmickProgressList.Count > 0)
        {
            var allGimmicks = Resources.FindObjectsOfTypeAll<GimmickBase>();
            foreach (var g in loadedData.gimmickProgressList)
            {
                var gimmick = allGimmicks.FirstOrDefault(x => x.gimmickID == g.gimmickID && x.gameObject.scene.isLoaded);
                if (gimmick != null)
                {
                    gimmick.LoadProgress(g.stage);
                    Debug.Log($"[GameBootstrap] �M�~�b�N {g.gimmickID} �� Stage={g.stage} �ɕ������܂���");
                }
                else
                {
                    Debug.LogWarning($"[GameBootstrap] gimmickID={g.gimmickID} �� Gimmick ��������܂���ł���");
                }
            }
        }

        // ===== �A�C�e���g���K�[�i�s�x�̕��� =====
        if (loadedData.itemTriggerList != null && loadedData.itemTriggerList.Count > 0)
        {
            var allTriggers = Resources.FindObjectsOfTypeAll<ItemTrigger>();
            foreach (var i in loadedData.itemTriggerList)
            {
                var trigger = allTriggers.FirstOrDefault(x => x.triggerID == i.triggerID && x.gameObject.scene.isLoaded);
                if (trigger != null)
                {
                    trigger.LoadProgress(i.currentStage);
                    Debug.Log($"[GameBootstrap] �A�C�e�� {i.triggerID} �� Stage={i.currentStage} �ɕ������܂���");
                }
                else
                {
                    Debug.LogWarning($"[GameBootstrap] triggerID={i.triggerID} �� ItemTrigger ��������܂���ł���");
                }
            }
        }

        Debug.Log("[GameBootstrap] �Z�[�u�f�[�^�̓K�p���������܂���");

        loadedData = null;
        Destroy(gameObject);
    }
}