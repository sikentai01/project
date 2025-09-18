using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    // �M�~�b�N�J�n�����i�q�N���X�ŕK����������j
    public abstract void StartGimmick(ItemTrigger trigger);

    // �M�~�b�N�������ɌĂԁi���ʏ����j
    protected void Complete(ItemTrigger trigger)
    {
        trigger.CompleteCurrentGimmick();
    }
}