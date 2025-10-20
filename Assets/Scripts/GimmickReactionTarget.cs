using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReactionEntry
{
    public string triggerID;             // 反応するトリガーID
    public string animationTriggerName;  // Animatorのトリガー名
}

public class GimmickReactionTarget : MonoBehaviour
{
    [Header("反応するトリガーとアニメーションの対応表")]
    public List<ReactionEntry> reactions = new List<ReactionEntry>();

    [Header("アニメーター設定")]
    public Animator targetAnimator;

    [Header("一度だけ再生する？")]
    public bool playOnce = true;

    private HashSet<string> playedIDs = new HashSet<string>();

    private void Awake()
    {
        if (targetAnimator == null)
            targetAnimator = GetComponent<Animator>();
    }

    // ItemTrigger などから呼び出される
    public void ReactToTrigger(string triggerID)
    {
        if (playOnce && playedIDs.Contains(triggerID))
            return;

        foreach (var entry in reactions)
        {
            if (entry.triggerID == triggerID)
            {
                if (targetAnimator != null && !string.IsNullOrEmpty(entry.animationTriggerName))
                {
                    targetAnimator.SetTrigger(entry.animationTriggerName);
                    Debug.Log($"[GimmickReactionTarget] {triggerID} に反応 → {entry.animationTriggerName} 再生");
                    playedIDs.Add(triggerID);
                }
                else
                {
                    Debug.LogWarning($"[GimmickReactionTarget] AnimatorまたはTrigger名が設定されていません ({triggerID})");
                }
                return; // 該当IDが見つかったら終了
            }
        }

        Debug.Log($"[GimmickReactionTarget] {triggerID} に対応するアニメが未登録");
    }
}