// NPC.cs
using UnityEngine;
public class NPC : MonoBehaviour
{
    public string npcName;

    public void ReceiveItem(ItemData item)
    {
        Debug.Log(npcName + " に " + item.itemName + " を渡した！");
        // イベント処理追加
    }
}
