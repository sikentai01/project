using UnityEngine;

public abstract class GimmickEffectBase : ItemEffect
{
    /// <summary>
    /// 対象のギミックを探して実行（共通処理）
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

        Debug.LogWarning($"[GimmickEffectBase] 対象のギミック {typeof(T).Name} が見つかりませんでした");
        return false;
    }
}