using UnityEngine;

// �K�{�v���Ƃ���Collider2D�������I�ɃA�^�b�`���܂�
[RequireComponent(typeof(Collider2D))]
public class EnterConversationTrigger : MonoBehaviour
{
    // ConversationTriggerAdapter �� ConversationHub �Ƃ̋��n�����ł�
    [SerializeField] private ConversationTriggerAdapter adapter;

    // �J�n�����b��ID�iConversationRouter.cs�Œ�`�j
    [SerializeField] private string conversationId = "sample_001";

    // �ڐG�𔻒肷��I�u�W�F�N�g�̃^�O�i�ʏ�� "Player"�j
    [SerializeField] private string requiredTag = "Player";

    // �� ������ KeyCode.Return (Enter�L�[) �ɕύX���܂�
    [SerializeField] private KeyCode key = KeyCode.Return;

    // �ڐG���ɃL�[���͂Ȃ��ő����ɊJ�n���邩�ǂ���
    [SerializeField] private bool autoStart = false;

    private bool inRange;
    private string lastColliderName;

    void Reset()
    {
        // Collider2D���g���K�[�ɐݒ�
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        // Adapter����������
        if (!adapter)
            adapter = ConversationTriggerAdapter.Instance
                      ?? FindObjectOfType<ConversationTriggerAdapter>(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;
        inRange = true;
        lastColliderName = other.name;

        if (autoStart)
            StartConv();
        else
            Debug.Log($"[EnterConversationTrigger] {other.name} ���͈͓� - {key}�L�[�ŊJ�n��");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(requiredTag)) return;
        inRange = false;
        Debug.Log($"[EnterConversationTrigger] {other.name} ���͈͊O");
    }

    void Update()
    {
        // �͈͓��ɂ��āA�������J�n�łȂ��ꍇ�ɃL�[���͂��`�F�b�N
        if (!inRange || autoStart) return;
        if (Input.GetKeyDown(key))
        {
            Debug.Log($"[EnterConversationTrigger] {key} ���� �� ��b�J�n");
            StartConv();
        }
    }

    void StartConv()
    {
        if (!adapter)
        {
            Debug.LogWarning($"[EnterConversationTrigger] Adapter���ݒ�");
            return;
        }

        // �O���ˑ��R�[�h: �v���C���[�ړ��ƃ��j���[�֎~
        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player ? player.GetComponent<GridMovement>() : null;
        if (move != null) move.enabled = false;

        // PauseMenu.blockMenu �͒�`����Ă��܂��񂪁A�����R�[�h�ɍ��킹�܂�
        // PauseMenu.blockMenu = true; 

        // ConversationHub.cs ���o�R���ĉ�b�J�n��ʒm
        if (string.IsNullOrWhiteSpace(conversationId))
        {
            // ID���Ȃ��ꍇ�́AAdapter�̃f�t�H���g�������Ăяo���z��
            // Debug.Log($"[EnterConversationTrigger] FireDefault() �Ăяo��");
            // adapter.FireDefault(); 
        }
        else
        {
            // ID���w�肵�ĊJ�n
            Debug.Log($"[EnterConversationTrigger] Fire({conversationId}) �Ăяo��");
            adapter.Fire(conversationId);
        }

        // ��b�I���C�x���g���w��
        if (DialogueCore.Instance != null)
        {
            DialogueCore.Instance.OnConversationEnded += OnConversationEnded;
        }
    }

    private void OnConversationEnded(string id)
    {
        // �I��������ړ��E���j���[��߂�
        // PauseMenu.blockMenu = false;

        var player = GameObject.FindGameObjectWithTag("Player");
        var move = player ? player.GetComponent<GridMovement>() : null;
        if (move != null) move.enabled = true;

        // �o�^����
        if (DialogueCore.Instance != null)
            DialogueCore.Instance.OnConversationEnded -= OnConversationEnded;

        Debug.Log($"[EnterConversationTrigger] ��b�I�� �� ����ĊJ ({id})");
    }
}