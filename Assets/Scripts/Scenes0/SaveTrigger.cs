using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

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

    [Header("NPC�֘A")]
    public GameObject sceneNpc;
    public Vector2 npcSpawnPosition;

    [Header("1�񂫂�ɂ��邩")]
    public bool oneTimeOnly = true;
    private bool alreadyTriggered = false;

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
        }
    }

    private IEnumerator EventFlow()
    {
        if (SoundManager.Instance != null && eventBGM != null)
        {
            SoundManager.Instance.PlayBGM(eventBGM);
        }

        alreadyTriggered = true;
        Debug.Log("�Z�[�u���ׂ�");

        if (restrictedLight != null) restrictedLight.enabled = false;
        if (normalLight != null) normalLight.enabled = true;

        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;

        if (player != null) player.SetDirection(0);

        if (sceneNpc != null)
        {
            sceneNpc.transform.position = npcSpawnPosition;
            sceneNpc.SetActive(true);
        }

        yield return new WaitForSeconds(1.5f);

        if (rewardItem != null)
        {
            InventoryManager.Instance.AddItem(rewardItem);
            Debug.Log($"�A�C�e���w{rewardItem.itemName}�x�����I");
        }

        yield return new WaitForSeconds(2f);

        if (sceneNpc != null) sceneNpc.SetActive(false);

        yield return new WaitForSeconds(1f);

        //  �t���O�o�^�i�Z�[�u�ς݁j
        GameFlags.Instance.SetFlag("SaveTriggered");


        if (player != null) player.enabled = true;
        PauseMenu.blockMenu = false;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBGM();
        }
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
