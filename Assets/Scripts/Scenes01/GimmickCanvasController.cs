using UnityEngine;
using TMPro; // TextMeshPro���g�p���邽��
using System;

public class GimmickCanvasController : MonoBehaviour
{
    // �� Button (2) - Button (3) - Button (4) - Button �� Text (TMP) �ւ̎Q�Ƃ�z�� ��
    [Header("�{�^���e�L�X�g�̎Q��")]
    public TMP_Text[] buttonLabels = new TMP_Text[4];

    [Header("�����e�L�X�g�p�l��")]
    public TMP_Text centerTextPanel; // �� ��b�p�l���̑���ɒ����p�l���Ƃ��Ďg�p

    [Header("�A������{�^���V�[�P���X�M�~�b�N")]
    public ButtonSequenceGimmick sequenceGimmick;

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
    }

    /// <summary>
    /// �����e�L�X�g�p�l���Ƀ��b�Z�[�W��ݒ肷��i��b�V�X�e�����p�Ȃ��j
    /// </summary>
    public void SetCenterMessage(string message)
    {
        if (centerTextPanel != null)
        {
            centerTextPanel.text = message;
        }
    }

    /// <summary>
    /// �w�肳�ꂽ�C���f�b�N�X�̃{�^���Ƀe�L�X�g��ݒ肷��
    /// </summary>
    public void SetButtonText(int index, string text)
    {
        if (buttonLabels.Length > index && buttonLabels[index] != null)
        {
            buttonLabels[index].text = text;
        }
    }
    // ... (OnAnyButtonClick, HideCanvas �͂��̂܂�) ...

    /// <summary>
    /// �{�^���N���b�N�C�x���g���M�~�b�N�ɓ]������ (Unity�C�x���g����ݒ肷��)
    /// </summary>
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

    public void HideCanvas()
    {
        gameObject.SetActive(false);
    }
}