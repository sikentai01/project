// NPC.cs
using UnityEngine;
public class NPC : MonoBehaviour
{
    public string npcName;

    public void ReceiveItem(ItemData item)
    {
        Debug.Log(npcName + " �� " + item.itemName + " ��n�����I");
        // �C�x���g�����ǉ�
    }
}
