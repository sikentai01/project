using UnityEngine;
using UnityEngine.UI;

public class GimmickCanvasController : MonoBehaviour
{
    public static GimmickCanvasController Instance { get; private set; }

    [Header("�L�����o�X�{��")]
    public Canvas canvasRoot;

    [Header("�{�^���Q�i�C���X�y�N�^�[�Ŋ��蓖�āj")]
    public Button[] buttons;

    // ���݂��̃L�����o�X���g�p���̃M�~�b�N
    private ButtonSequenceGimmick activeGimmick;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (canvasRoot == null)
            canvasRoot = GetComponentInChildren<Canvas>();

        HideCanvas();
    }

    // ===============================
    //  ��{�\������
    // ===============================
    public void ShowCanvas(ButtonSequenceGimmick gimmick)
    {
        activeGimmick = gimmick;

        if (canvasRoot != null)
            canvasRoot.enabled = true;

        // �{�^���������̃��X�i�[�ݒ�
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() =>
            {
                OnButtonPressed(index);
            });
        }
    }

    public void HideCanvas()
    {
        if (canvasRoot != null)
            canvasRoot.enabled = false;

        activeGimmick = null;
    }

    // ===============================
    //  �{�^������
    // ===============================
    private void OnButtonPressed(int index)
    {
        // �R�Â��Ă���M�~�b�N�ɒʒm
        if (activeGimmick != null)
        {
            activeGimmick.OnButtonClick(index);
        }
        else
        {
            Debug.LogWarning("[GimmickCanvasController] �A�N�e�B�u�ȃM�~�b�N�����ݒ�ł��B");
        }
    }

    /// <summary>
    /// �w��C���f�b�N�X�̃{�^���Ƀe�L�X�g��ݒ�
    /// </summary>
    public void SetButtonText(int index, string text)
    {
        if (buttons == null || index < 0 || index >= buttons.Length)
        {
            Debug.LogWarning($"[GimmickCanvasController] �����ȃ{�^���C���f�b�N�X: {index}");
            return;
        }

        var tmp = buttons[index].GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = text;
        else
            Debug.LogWarning($"[GimmickCanvasController] �{�^�� {index} �� TextMeshProUGUI ��������܂���B");
    }

    /// <summary>
    /// ���ׂẴ{�^���e�L�X�g���ꊇ�Őݒ�
    /// </summary>
    public void SetAllButtonTexts(string[] texts)
    {
        if (buttons == null) return;

        for (int i = 0; i < buttons.Length && i < texts.Length; i++)
        {
            SetButtonText(i, texts[i]);
        }
    }
}
