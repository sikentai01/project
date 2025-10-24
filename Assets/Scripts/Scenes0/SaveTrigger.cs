using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SaveTrigger : MonoBehaviour, ISceneInitializable
{
    [Header("�T�E���h�ݒ�")]
    [SerializeField] private AudioClip eventBGM;

    private bool isPlayerNear = false;
    private GridMovement player;

    [Header("�K�v�ȕ��� (0=��, 1=��, 2=�E, 3=��, -1=�����Ȃ�)")]
    public int requiredDirection = 3;

    [Header("NPC�֘A�i�V�[�����̉��u���p�j")]
    public GameObject sceneNpc;
    public Vector2 npcSpawnPosition;

    [Header("1�񂫂�ɂ��邩")]
    public bool oneTimeOnly = true;
    private bool alreadyTriggered = false;

    private const string FLAG_ID = "SaveTriggered";

    private void OnEnable()
    {
        StartCoroutine(DeferredFlagSync());
    }

    private IEnumerator DeferredFlagSync()
    {
        yield return null; // GameBootstrap�̃��[�h������1�t���[���҂�
        if (GameFlags.Instance != null)
        {
            alreadyTriggered = GameFlags.Instance.HasFlag(FLAG_ID);
            Debug.Log($"[SaveTrigger] DeferredFlagSync: {FLAG_ID}={alreadyTriggered}");
        }
    }

    private void Start()
    {
        InitializeTrigger();
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            if (oneTimeOnly && alreadyTriggered) return;

            if (requiredDirection == -1 || player.GetDirection() == requiredDirection)
                StartCoroutine(EventFlow());
            else
                Debug.Log("[SaveTrigger] �������Ⴄ�̂Œ��ׂ��Ȃ�");
        }
    }

    private IEnumerator EventFlow()
    {
        alreadyTriggered = true;
        Debug.Log("[SaveTrigger] �C�x���g�J�n");

        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;
        // --- �Z�[�uUI���J���i�{����p���[�h�j ---
        if (SaveSlotUIManager.Instance != null)
        {
            SaveSlotUIManager.Instance.OpenViewOnlyPanel();
            Debug.Log("[SaveTrigger] �{����p�Z�[�u�X���b�g���J���܂����B");

            //  �v���C���[������܂őҋ@
            yield return new WaitUntil(() => !SaveSlotUIManager.Instance.IsOpen());
            Debug.Log("[SaveTrigger] �Z�[�u�X���b�g�������܂����B");
        }
        PauseMenu.blockMenu = true;
        player.SetDirection(0);
        SoundManager.Instance?.PlayBGM(eventBGM);

        if (sceneNpc != null)
        {
            sceneNpc.transform.position = npcSpawnPosition;
            sceneNpc.SetActive(true);
        }

        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire("talk_001");
            yield return new WaitUntil(() => !IsConversationActive());
        }

        if (sceneNpc != null) sceneNpc.SetActive(false);

        var trap = FindFirstObjectByType<FallTrap>();
        trap?.DisableTrap();

        GameFlags.Instance?.SetFlag(FLAG_ID);
        GameFlags.Instance?.SetFlag("DiaryEventUnlocked");

        if (player != null) player.enabled = true;
        PauseMenu.blockMenu = false;
        SoundManager.Instance?.StopBGM();

        Debug.Log("[SaveTrigger] �C�x���g�I��");
    }

    private bool IsConversationActive()
    {
        var core = FindObjectOfType<DialogueCore>();
        return core != null && core.enabled;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNear = false;
    }

    private void InitializeTrigger()
    {
        player = FindFirstObjectByType<GridMovement>();
        if (player == null) return;

        if (sceneNpc != null) sceneNpc.SetActive(false);

        Debug.Log($"[SaveTrigger] ���������� (Triggered={alreadyTriggered})");
    }

    public void InitializeSceneAfterLoad()
    {
        Debug.Log("[SaveTrigger] InitializeSceneAfterLoad �Ăяo��");
        InitializeTrigger();
    }
}