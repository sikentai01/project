using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    // �M�~�b�N�J�n�����i�q�N���X�Ŏ�������j
    public abstract void StartGimmick(ItemTrigger trigger);

    // �M�~�b�N�������ɌĂ�
    protected void Complete(ItemTrigger trigger)
    {
        trigger.CompleteCurrentGimmick();
    }
}