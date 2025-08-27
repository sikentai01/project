using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;            // アイテム名
    [TextArea] public string description; // 説明文
    public Sprite icon;                // アイコン画像（必要なければ省略可）
}
