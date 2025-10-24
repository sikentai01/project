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

    // �M�~�b�N�N���Ɏg�p�����bID
    [Header("�A�C�e���g�p���ɊJ�n�����bID")]
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
                Debug.LogWarning($"[ExchangeItemEffect] ����������������܂���B (�K�v: {requiredDirection}, ����: {movement.GetDirection()})");
                return false;
            }
        }

        // 2. �R���C�_�[�͈͂̃`�F�b�N
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
        if (gimmick == null || DialogueCore.Instance == null || string.IsNullOrEmpty(conversationId))
        {
            Debug.LogWarning("[ExchangeItemEffect] �M�~�b�N/Core/��bID�̂����ꂩ���s�����Ă��܂��B");
            return;
        }

        if (activeCallbacks.ContainsKey(conversationId))
        {
            DialogueCore.Instance.OnConversationEnded -= activeCallbacks[conversationId];
            activeCallbacks.Remove(conversationId);
        }

        // 3. ��b�I�����Ɏ��s����A�N�V�������`
        Action<string> onConvEnd = null;
        onConvEnd = (finishedId) =>
        {
            if (finishedId != conversationId) return;

            // �M�~�b�N�N���ASE�Đ��A�A�C�e���폜
            if (gimmick.ExecuteExchange())
            {
                // ������ CS0571 �G���[�C���ӏ� ������
                if (SoundManager.Instance != null && successSeClip != null)
                {
                    SoundManager.Instance.PlaySE(successSeClip);
                    Debug.Log($"[ExchangeItemEffect] ��b�I���� SE�Đ�: {successSeClip.name}");
                }

                // �A�C�e���폜
                InventoryManager.Instance.RemoveItemByID(usedItem.itemID);
            }

            // ���X�i�[����
            DialogueCore.Instance.OnConversationEnded -= onConvEnd;
            activeCallbacks.Remove(conversationId);
            Debug.Log($"[ExchangeItemEffect] ��b�I���������� ({conversationId})");
        };

        // 4. �C�x���g���X�i�[��o�^
        DialogueCore.Instance.OnConversationEnded += onConvEnd;
        activeCallbacks.Add(conversationId, onConvEnd);

        // 5. ��b���J�n (ConversationHub�o�R)
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