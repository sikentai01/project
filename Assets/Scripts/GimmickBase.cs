using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    public GimmickBase nextGimmick;

    public abstract void StartGimmick(ItemTrigger trigger);

    protected void Complete(ItemTrigger trigger)
    {
        if (nextGimmick != null)
            nextGimmick.StartGimmick(trigger);
        else
            trigger.CollectItem(); // ÅŒã‚È‚çƒAƒCƒeƒ€“üè
    }
}