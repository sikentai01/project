using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    // ギミック開始処理（子クラスで実装する）
    public abstract void StartGimmick(ItemTrigger trigger);

    // ギミック完了時に呼ぶ
    protected void Complete(ItemTrigger trigger)
    {
        trigger.CompleteCurrentGimmick();
    }
}