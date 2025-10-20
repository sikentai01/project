using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ロード直後にセーブデータをゲームへ反映するクラス。
/// SaveSlotButton から生成され、適用後に自壊します。
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    public static SaveSystem.SaveData loadedData; // SaveSlotButton から代入される

    private IEnumerator Start()
    {
        if (loadedData == null)
        {
            Debug.LogWarning("[GameBootstrap] loadedData がありません。何も適用しません。");
            Destroy(gameObject);
            yield break;
        }

        // 1フレーム待ってシーン内オブジェクトの生成完了を待つ
        yield return null;

        // ===== インベントリ・ドキュメント・フラグ適用 =====
        if (InventoryManager.Instance != null && loadedData.inventoryData != null)
            InventoryManager.Instance.LoadData(loadedData.inventoryData);

        if (DocumentManager.Instance != null && loadedData.documentData != null)
            DocumentManager.Instance.LoadData(loadedData.documentData);

        if (GameFlags.Instance != null && loadedData.flagData != null)
            GameFlags.Instance.LoadFlags(loadedData.flagData);

        // ===== プレイヤー位置・向き =====
        var player = Object.FindFirstObjectByType<GridMovement>();
        if (player != null)
        {
            player.transform.position = loadedData.playerPosition;
            player.SetDirection(loadedData.playerDirection);
        }
        else
        {
            Debug.LogWarning("[GameBootstrap] プレイヤーが見つかりません（位置・向き適用スキップ）");
        }

        // ===== ギミック進行度の復元 =====
        if (loadedData.gimmickProgressList != null && loadedData.gimmickProgressList.Count > 0)
        {
            var gimmicks = Object.FindObjectsByType<GimmickBase>(FindObjectsSortMode.None);
            foreach (var g in loadedData.gimmickProgressList)
            {
                var gimmick = gimmicks.FirstOrDefault(x => x.gimmickID == g.gimmickID);
                if (gimmick != null)
                {
                    gimmick.LoadProgress(g.stage);
                    Debug.Log($"[GameBootstrap] ギミック {g.gimmickID} を Stage={g.stage} に復元しました");
                }
                else
                {
                    Debug.LogWarning($"[GameBootstrap] gimmickID={g.gimmickID} の Gimmick が見つかりませんでした");
                }
            }
        }

        // ===== アイテムトリガー進行度の復元 =====
        if (loadedData.itemTriggerList != null && loadedData.itemTriggerList.Count > 0)
        {
            var itemTriggers = Object.FindObjectsByType<ItemTrigger>(FindObjectsSortMode.None);
            foreach (var i in loadedData.itemTriggerList)
            {
                var trigger = itemTriggers.FirstOrDefault(x => x.triggerID == i.triggerID);
                if (trigger != null)
                {
                    trigger.LoadProgress(i.currentStage);
                    Debug.Log($"[GameBootstrap] アイテム {i.triggerID} を Stage={i.currentStage} に復元しました");
                }
                else
                {
                    Debug.LogWarning($"[GameBootstrap] triggerID={i.triggerID} の ItemTrigger が見つかりませんでした");
                }
            }
        }

        Debug.Log("[GameBootstrap] セーブデータの適用が完了しました");

        // 片付け
        loadedData = null;
        Destroy(gameObject);
    }
}