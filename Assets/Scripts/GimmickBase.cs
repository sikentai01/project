using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    // ギミック開始処理（子クラスで必ず実装する）
    public abstract void StartGimmick(ItemTrigger trigger);

    // ギミック完了時に呼ぶ（共通処理）
    protected void Complete(ItemTrigger trigger)
    {
        trigger.CompleteCurrentGimmick();
    }
}