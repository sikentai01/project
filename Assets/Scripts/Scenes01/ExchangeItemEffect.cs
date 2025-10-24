using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Gimmick/Exchange Item Effect", fileName = "ExchangeItemEffect")]
public class ExchangeItemEffect : GimmickEffectBase
{
    [Header("���삷��M�~�b�NID (�C�ӁF�߂��ɕ����̃M�~�b�N������ꍇ)")]
    public string targetGimmickID;

    [Header("�K�v�Ȍ��� (0=��, 1=��, 2=�E, 3=��, -1=�����Ȃ�)")]
    public int requiredDirection = -1;

    [Header("�A�C�e���g�p���ɊJ�n�����bID�i�󗓂Ȃ��b�Ȃ��ő����s�j")]
    public string conversationId = "";

    [Header("�M�~�b�N�������ɍĐ�����SE")]
    public AudioClip successSeClip;

    private static Dictionary<string, Action<string>> activeCallbacks = new Dictionary<string, Action<string>>();

    // CanExecute() �̃��W�b�N�͕ύX�Ȃ�

    public override bool CanExecute(ItemData item)
    {
        var player = UnityEngine.GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;

        // 1. �����̃`�F�b�N
        if (requiredDirection != -1)
        {
            var movement = player.GetComponent<GridMovement>();
            if (movement == null) return false;

            if (movement.GetDirection() != requiredDirection)
            {
                // �v���C���[�̌������Ⴄ�ꍇ�A�x�����o������ false
                return false;
            }
        }

        // 2. �M�~�b�N�̑��݃`�F�b�N
        var triggers = UnityEngine.Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            if (!trigger.IsPlayerNear) continue;

            var gimmick = trigger.GetGimmick<ItemExchangeGimmick>();
            if (gimmick != null)
            {
                if (string.IsNullOrEmpty(targetGimmickID) || gimmick.gimmickID == targetGimmickID)
                {
                    return true;
                }
            }
        }

        Debug.LogWarning($"[ExchangeItemEffect] GimmickTrigger�̃R���C�_�[�͈͊O���A�ΏۃM�~�b�N��������܂���B");
        return false;
    }


    public override void Execute(ItemData usedItem)
    {
        var gimmick = FindNearbyGimmick<ItemExchangeGimmick>();

        if (gimmick == null)
        {
            Debug.LogWarning("[ExchangeItemEffect] �M�~�b�N��������܂���B���s�ł��܂���B");
            return;
        }

        bool hasConversation = !string.IsNullOrEmpty(conversationId);

        if (hasConversation)
        {
            // --- ��b����̏ꍇ: ��b�J�n�ƏI���҂� ---

            if (DialogueCore.Instance == null)
            {
                Debug.LogWarning("[ExchangeItemEffect] DialogueCore��������܂���B��b���X�L�b�v���A���ڎ��s���܂��B");
                // Core���Ȃ��ꍇ�͉�b�Ȃ��Ɠ��������փt�H�[���o�b�N
                ExecuteGimmickAndFinish(gimmick, usedItem);
                return;
            }

            // 1. �ȑO�̃��X�i�[������
            if (activeCallbacks.ContainsKey(conversationId))
            {
                DialogueCore.Instance.OnConversationEnded -= activeCallbacks[conversationId];
                activeCallbacks.Remove(conversationId);
            }

            // 2. ��b�I�����Ɏ��s����A�N�V�������`
            Action<string> onConvEnd = null;
            onConvEnd = (finishedId) =>
            {
                if (finishedId != conversationId) return;
                ExecuteGimmickAndFinish(gimmick, usedItem); // �M�~�b�N���s

                // ���X�i�[����
                DialogueCore.Instance.OnConversationEnded -= onConvEnd;
                activeCallbacks.Remove(conversationId);
            };

            // 3. �C�x���g���X�i�[��o�^���A��b���J�n
            DialogueCore.Instance.OnConversationEnded += onConvEnd;
            activeCallbacks.Add(conversationId, onConvEnd);

            if (ConversationHub.Instance != null)
            {
                ConversationHub.Instance.Fire(conversationId);
                Debug.Log($"[ExchangeItemEffect] ��b�J�n: {conversationId}");
            }
            else
            {
                Debug.LogWarning("[ExchangeItemEffect] ConversationHub��������Ȃ����߉�b���J�n�ł��܂���B");
            }
        }
        else
        {
            // --- ��b�Ȃ��̏ꍇ: �����ɃM�~�b�N���s ---
            ExecuteGimmickAndFinish(gimmick, usedItem);
        }
    }

    // ��b�Ȃ�/��b�I����Ɏ��s����鋤�ʏ���
    private void ExecuteGimmickAndFinish(ItemExchangeGimmick gimmick, ItemData usedItem)
    {
        if (gimmick.ExecuteExchange())
        {
            // SE�Đ�
            if (SoundManager.Instance != null && successSeClip != null)
            {
                SoundManager.Instance.PlaySE(successSeClip);
                Debug.Log($"[ExchangeItemEffect] �M�~�b�N���� SE�Đ�: {successSeClip.name}");
            }

            // �A�C�e���폜
            InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
        }
        Debug.Log("[ExchangeItemEffect] �M�~�b�N���s���������B");
    }

    // �߂��̃M�~�b�N��������w���p�[
    private T FindNearbyGimmick<T>() where T : GimmickBase
    {
        var triggers = UnityEngine.Object.FindObjectsByType<GimmickTrigger>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            if (!trigger.IsPlayerNear) continue;

            var gimmick = trigger.GetGimmick<T>();
            if (gimmick != null)
            {
                if (string.IsNullOrEmpty(targetGimmickID) || gimmick.gimmickID == targetGimmickID)
                {
                    return gimmick;
                }
            }
        }
        return null;
    }
}