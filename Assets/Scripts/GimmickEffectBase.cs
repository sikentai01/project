using UnityEngine;

public abstract class GimmickEffectBase : ItemEffect
{
    /// <summary>
    /// �Ώۂ̃M�~�b�N��T���Ď��s�i���ʏ����j
    /// </summary>
    protected bool TryInvokeNearbyGimmick<T>(System.Action<T> onFound) where T : GimmickBase
    {
        var triggers = Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            if (!trigger.IsPlayerNear) continue;

            var gimmick = trigger.GetGimmick<T>();
            if (gimmick != null)
            {
                onFound?.Invoke(gimmick);
                return true;
            }
        }

        Debug.LogWarning($"[GimmickEffectBase] �Ώۂ̃M�~�b�N {typeof(T).Name} ��������܂���ł���");
        return false;
    }
}