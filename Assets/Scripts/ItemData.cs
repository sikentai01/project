using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;        // �A�C�e����
    [TextArea] public string description; // ������
    public bool isConsumable;      // ���Օi���ǂ���

    // ���ʂ̎�ށi�K�v�ɉ����đ��₹��j
    public enum EffectType { None, Key, Poison, Water, Lubricant }
    public EffectType effectType;

    // ����ID�i�������ƂɈႤ�l��ݒ肵�ē���̃h�A�����J������悤�ɂ���j
    public string keyID;

    // �A�C�e�����g�����Ƃ��̏���
    public void UseEffect()
    {
        Debug.Log(itemName + " �̌��ʂ𔭓��I");

        switch (effectType)
        {
            case EffectType.Key:
                // ���̓h�A�X�N���v�g���Ŕ��肷��
                Debug.Log("�h�A�̑O�Ŏg���Ă�������");
                break;

            case EffectType.Poison:
                Debug.Log("�ł�����ł��܂����c�Q�[���I�[�o�[�I");
                // GameManager.Instance.GameOver(); �� �����ŃQ�[���I�[�o�[������
                break;

            case EffectType.Water:
                Debug.Log("�����̂Ă�");
                break;

            case EffectType.Lubricant:
                Debug.Log("�K�𗎂Ƃ����I");
                break;

            default:
                Debug.Log("���ʂȌ��ʂ͂Ȃ�");
                break;
        }
    }
}