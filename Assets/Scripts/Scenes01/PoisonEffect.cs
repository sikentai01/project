using UnityEngine;

[CreateAssetMenu(menuName = "Game/Effects/Poisoning Effect", fileName = "PoisoningEffect")]
public class PoisoningEffect : ItemEffect
{
    // [Header("使用後にアクティブにするGameOverコントローラー")]
    // public GameObject gameOverControllerObject; // ★ このフィールドは不要になります

    [Header("使用時にインベントリから削除するか")]
    public bool isConsumable = true;

    /// <summary>
    /// アイテム使用の条件（常時使用可能とする）
    /// </summary>
    public override bool CanExecute(ItemData item)
    {
        // 毒瓶はどこでも使えるが、結果としてゲームオーバーになるという設定
        return true;
    }

    /// <summary>
    /// 実行：ゲームオーバーを起動し、アイテムを削除する
    /// </summary>
    public override void Execute(ItemData usedItem)
    {
        // 1. アイテムをインベントリから削除
        if (isConsumable && InventoryManager.Instance != null)
        {
            // ItemDataには itemID があると想定し、それを使って削除する
            InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            Debug.Log($"[PoisoningEffect] {usedItem.itemName} を使用し、削除しました。");
        }

        // 2. ゲームオーバー処理の実行 (BootLoaderによるシーン切り替え)
        if (BootLoader.Instance != null)
        {
            // BootLoader経由でGameOverシーンに即座に切り替える
            BootLoader.Instance.SwitchSceneInstant("GameOver");
            Debug.Log("[PoisoningEffect] 毒瓶を使用 → GameOverシーンへ即時切り替え");
        }
        else
        {
            // BootLoaderがない場合は、従来の処理の代替ログを出力
            Debug.LogError("[PoisoningEffect] BootLoader.Instanceが見つかりません。GameOver処理をスキップしました。");
        }
    }
}