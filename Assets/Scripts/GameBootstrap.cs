using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ���[�h����ɃZ�[�u�f�[�^���Q�[���֔��f����K�p�N���X�B
/// SaveSlotButton ���琶������A�K�p��Ɏ��󂵂܂��B
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

        // 1�t���[���҂��ăV�[�����I�u�W�F�N�g�̐���������҂�
        yield return null;

        // ===== �C���x���g���E�h�L�������g�E�t���O�K�p =====
        if (InventoryManager.Instance != null && loadedData.inventoryData != null)
            InventoryManager.Instance.LoadData(loadedData.inventoryData);

        if (DocumentManager.Instance != null && loadedData.documentData != null)
            DocumentManager.Instance.LoadData(loadedData.documentData);

        if (GameFlags.Instance != null && loadedData.flagData != null)
            GameFlags.Instance.LoadFlags(loadedData.flagData);

        // ===== �v���C���[�ʒu�E���� =====
        var player = Object.FindFirstObjectByType<GridMovement>();
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
            var triggers = Object.FindObjectsByType<ItemTrigger>(FindObjectsSortMode.None);
            foreach (var g in loadedData.gimmickProgressList)
            {
                var t = triggers.FirstOrDefault(x => x.triggerID == g.triggerID);
                if (t != null)
                {
                    t.LoadProgress(g.stage);
                }
                else
                {
                    Debug.LogWarning($"[GameBootstrap] triggerID={g.triggerID} �� ItemTrigger ��������܂���ł���");
                }
            }
        }

        Debug.Log("[GameBootstrap] �Z�[�u�f�[�^�̓K�p���������܂���");

        // �Еt��
        loadedData = null;
        Destroy(gameObject);
    }
}