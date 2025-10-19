using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SaveTrigger : MonoBehaviour
{
    [Header("�T�E���h�ݒ�")]
    [SerializeField] private AudioClip eventBGM;

    private bool isPlayerNear = false;
    private GridMovement player;

    [Header("�K�v�ȕ��� (0=��, 1=��, 2=�E, 3=��, -1=�����Ȃ�)")]
    public int requiredDirection = 3;

    [Header("��b�C�x���g�œn���A�C�e��")]
    public ItemData rewardItem;

    private Light2D normalLight;
    private Light2D restrictedLight;

    [Header("NPC�֘A�i�V�[�����̉��u���p�j")]
    public GameObject sceneNpc;
    public Vector2 npcSpawnPosition;

    [Header("1�񂫂�ɂ��邩")]
    public bool oneTimeOnly = true;
    private bool alreadyTriggered = false;

    void OnEnable()
    {
        if (GameFlags.Instance != null && !GameFlags.Instance.HasFlag("SaveTriggered"))
        {
            alreadyTriggered = false;
            Debug.Log("[SaveTrigger] �t���O���ݒ�̂��ߍėL����");
        }
    }

    void Start()
    {
        player = FindFirstObjectByType<GridMovement>();

        if (player != null)
        {
            normalLight = player.GetComponent<Light2D>();
            var childLights = player.GetComponentsInChildren<Light2D>(true);
            foreach (var l in childLights)
            {
                if (l.name == "RestrictedLight")
                    restrictedLight = l;
            }

            if (restrictedLight != null) restrictedLight.enabled = true;
            if (normalLight != null) normalLight.enabled = false;
        }

        if (sceneNpc != null) sceneNpc.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
            if (oneTimeOnly && alreadyTriggered) return;

            if (requiredDirection == -1 || player.GetDirection() == requiredDirection)
            {
                StartCoroutine(EventFlow());
            }
            else
            {
                Debug.Log("�������Ⴄ�̂Œ��ׂ��Ȃ�");
            }
        }
    }

    private IEnumerator EventFlow()
    {
        alreadyTriggered = true;
        Debug.Log("[SaveTrigger] �C�x���g�J�n");

        // ---- BGM�Đ� ----
        if (SoundManager.Instance != null && eventBGM != null)
            SoundManager.Instance.PlayBGM(eventBGM);

        // ---- ���C�g�؂�ւ� ----
        if (restrictedLight != null) restrictedLight.enabled = false;
        if (normalLight != null) normalLight.enabled = true;

        // ---- �v���C���[��~ & ���j���[�֎~ ----
        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;
        player?.SetDirection(0);

        // ---- NPC�o�� ----
        if (sceneNpc != null)
        {
            sceneNpc.transform.position = npcSpawnPosition;
            sceneNpc.SetActive(true);
        }

        // ----  ��b�C�x���g���Đ� ----
        if (ConversationHub.Instance != null)
        {
            ConversationHub.Instance.Fire("talk_001");
            yield return new WaitUntil(() => !IsConversationActive());
        }

        // ---- ��b�I����ɃA�C�e���n�� ----
        if (rewardItem != null)
        {
            InventoryManager.Instance.AddItem(rewardItem);
            Debug.Log($"[SaveTrigger] �A�C�e���w{rewardItem.itemName}�x�����I");
        }

        // ---- NPC������ ----
        if (sceneNpc != null) sceneNpc.SetActive(false);

        // ---- ���Ƃ��������� ----
        var trap = FindFirstObjectByType<FallTrap>();
        if (trap != null) trap.DisableTrap();

        // ---- ���A���� ----
        GameFlags.Instance.SetFlag("SaveTriggered");
        if (player != null) player.enabled = true;
        PauseMenu.blockMenu = false;

        if (SoundManager.Instance != null)
            SoundManager.Instance.StopBGM();

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
}