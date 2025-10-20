using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReactionEntry
{
    public string triggerID;             // ��������g���K�[ID
    public string animationTriggerName;  // Animator�̃g���K�[��
}

public class GimmickReactionTarget : MonoBehaviour
{
    [Header("��������g���K�[�ƃA�j���[�V�����̑Ή��\")]
    public List<ReactionEntry> reactions = new List<ReactionEntry>();

    [Header("�A�j���[�^�[�ݒ�")]
    public Animator targetAnimator;

    [Header("��x�����Đ�����H")]
    public bool playOnce = true;

    private HashSet<string> playedIDs = new HashSet<string>();

    private void Awake()
    {
        if (targetAnimator == null)
            targetAnimator = GetComponent<Animator>();
    }

    // ItemTrigger �Ȃǂ���Ăяo�����
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
                    Debug.Log($"[GimmickReactionTarget] {triggerID} �ɔ��� �� {entry.animationTriggerName} �Đ�");
                    playedIDs.Add(triggerID);
                }
                else
                {
                    Debug.LogWarning($"[GimmickReactionTarget] Animator�܂���Trigger�����ݒ肳��Ă��܂��� ({triggerID})");
                }
                return; // �Y��ID������������I��
            }
        }

        Debug.Log($"[GimmickReactionTarget] {triggerID} �ɑΉ�����A�j�������o�^");
    }
}