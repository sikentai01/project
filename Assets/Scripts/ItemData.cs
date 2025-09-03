using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;        // アイテム名
    [TextArea] public string description; // 説明文
    public bool isConsumable;      // 消耗品かどうか

    // 効果の種類（必要に応じて増やせる）
    public enum EffectType { None, Key, Poison, Water, Lubricant }
    public EffectType effectType;

    // 鍵のID（部屋ごとに違う値を設定して特定のドアだけ開けられるようにする）
    public string keyID;

    // アイテムを使ったときの処理
    public void UseEffect()
    {
        Debug.Log(itemName + " の効果を発動！");

        switch (effectType)
        {
            case EffectType.Key:
                // 鍵はドアスクリプト側で判定する
                Debug.Log("ドアの前で使ってください");
                break;

            case EffectType.Poison:
                Debug.Log("毒を飲んでしまった…ゲームオーバー！");
                // GameManager.Instance.GameOver(); ← ここでゲームオーバー処理へ
                break;

            case EffectType.Water:
                Debug.Log("水を捨てた");
                break;

            case EffectType.Lubricant:
                Debug.Log("錆を落とした！");
                break;

            default:
                Debug.Log("特別な効果はない");
                break;
        }
    }
}