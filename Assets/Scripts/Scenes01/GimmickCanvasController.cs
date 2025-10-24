using UnityEngine;
using TMPro; // TextMeshPro���g�p���邽��
using System;

public class GimmickCanvasController : MonoBehaviour
{
    // �� Button (2) - Button (3) - Button (4) - Button �� Text (TMP) �ւ̎Q�Ƃ�z�� ��
    [Header("�{�^���e�L�X�g�̎Q��")]
    [Tooltip("GimmickCanvas����4�̃{�^����Text (TMP) �R���|�[�l���g�����Ɋ��蓖�ĂĂ��������B")]
    public TMP_Text[] buttonLabels = new TMP_Text[4];

    [Header("��b�\���p�e�L�X�g�p�l��")]
    [Tooltip("DialogueCore����e�L�X�g���󂯎��\������TMP_Text")]
    public TMP_Text conversationTextPanel;

    [Header("�A������{�^���V�[�P���X�M�~�b�N")]
    public ButtonSequenceGimmick sequenceGimmick; // �M�~�b�N�{�̂ւ̎Q��

    private static GimmickCanvasController instance;
    public static GimmickCanvasController Instance => instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // DialogueCore�̃C�x���g�ɐڑ�
        if (DialogueCore.Instance != null && conversationTextPanel != null)
        {
            // Core���e�L�X�g�����������Ƃ��Ƀe�L�X�g�p�l�����X�V
            DialogueCore.Instance.OnLinesReady += SetConversationPanel;
            // ��b�I�����Ƀp�l�����N���A (�I�v�V����)
            DialogueCore.Instance.OnConversationEnded += _ => conversationTextPanel.text = "";
        }
    }

    void OnDestroy()
    {
        if (DialogueCore.Instance != null && conversationTextPanel != null)
        {
            // �w�ǉ���
            DialogueCore.Instance.OnLinesReady -= SetConversationPanel;
            DialogueCore.Instance.OnConversationEnded -= _ => conversationTextPanel.text = "";
        }
    }

    /// <summary>
    /// DialogueCore����󂯎������b�s���e�L�X�g�p�l���ɕ\������ (���p�@�\)
    /// </summary>
    private void SetConversationPanel(string[] lines)
    {
        if (conversationTextPanel != null)
        {
            // ��b�s���������A�����ɕ\��
            conversationTextPanel.text = string.Join("\n", lines);
        }
    }


    /// <summary>
    /// �w�肳�ꂽ�C���f�b�N�X�̃{�^���Ƀe�L�X�g��ݒ肷��
    /// </summary>
    /// <param name="index">�{�^���̃C���f�b�N�X (0�`3)</param>
    /// <param name="text">�ݒ肷��e�L�X�g</param>
    public void SetButtonText(int index, string text)
    {
        if (buttonLabels.Length > index && buttonLabels[index] != null)
        {
            buttonLabels[index].text = text;
            Debug.Log($"[GimmickCanvas] �{�^�� {index + 1} �Ƀe�L�X�g '{text}' ��ݒ肵�܂����B");
        }
        else
        {
            Debug.LogWarning($"[GimmickCanvas] �{�^���̃C���f�b�N�X {index} �͖������A�Q�Ƃ�����܂���B");
        }
    }

    /// <summary>
    /// �{�^���N���b�N�C�x���g���M�~�b�N�ɓ]������ (Unity�C�x���g����ݒ肷��)
    /// </summary>
    /// <param name="index">�N���b�N���ꂽ�{�^���̃C���f�b�N�X (0�`3)</param>
    public void OnAnyButtonClick(int index)
    {
        if (sequenceGimmick != null)
        {
            sequenceGimmick.OnButtonClick(index);
        }
        else
        {
            Debug.LogWarning("[GimmickCanvas] ButtonSequenceGimmick���ݒ肳��Ă��܂���B");
        }
    }

    /// <summary>
    /// �M�~�b�N�����������ۂ�Canvas���\���ɂ���
    /// </summary>
    public void HideCanvas()
    {
        // �M�~�b�N�����������ꍇ�ACanvas�S�̂��A�N�e�B�u�ɂ���
        gameObject.SetActive(false);
        Debug.Log("[GimmickCanvas] Canvas���\���ɂ��܂����B");
    }
}