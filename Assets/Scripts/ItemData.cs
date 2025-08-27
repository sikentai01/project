using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;            // �A�C�e����
    [TextArea] public string description; // ������
    public Sprite icon;                // �A�C�R���摜�i�K�v�Ȃ���Ώȗ��j
}
