using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SaveTrigger : MonoBehaviour
{
    private bool isPlayerNear = false;
    private GridMovement player;

    [Header("�K�v�ȕ��� (0=��, 1=��, 2=�E, 3=��, -1=�����Ȃ�)")]
    public int requiredDirection = 3;

    [Header("��b�C�x���g�œn���A�C�e��")]
    public ItemData rewardItem;

    // ���C�g�iInspector�Ńh���b�O�s�v�AStart�Ŏ����擾�j
    private Light2D normalLight;
    private Light2D restrictedLight;

    [Header("NPC�֘A�i�V�[�����̉��u���p�j")]
    public GameObject sceneNpc;        // �V�[�����ɒu������NPC
    public Vector2 npcSpawnPosition;   // Inspector�Œ��ڍ��W����

    // �����I��Prefab�ł��ꍇ
    // public GameObject npcPrefab;

    [Header("1�񂫂�ɂ��邩")]
    public bool oneTimeOnly = true;    // true�Ȃ�1�����
    private bool alreadyTriggered = false;

    void Start()
    {
        player = FindFirstObjectByType<GridMovement>();

        if (player != null)
        {
            // NormalLight �̓v���C���[�{�̂ɕt���Ă�
            normalLight = player.GetComponent<Light2D>();

            // RestrictedLight �̓v���C���[�̎q����擾
            var childLights = player.GetComponentsInChildren<Light2D>(true);
            foreach (var l in childLights)
            {
                if (l.name == "RestrictedLight")
                    restrictedLight = l;
            }

            // �J�n���� Restricted ����ON
            if (restrictedLight != null) restrictedLight.enabled = true;
            if (normalLight != null) normalLight.enabled = false;
        }

        // ���u��NPC�͍ŏ���\��
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
        alreadyTriggered = true; // 1�����ɂ���ꍇ�͂����Ń��b�N

        Debug.Log("�Z�[�u���ׂ�");

        // ���C�g�؂�ւ�
        if (restrictedLight != null) restrictedLight.enabled = false;
        if (normalLight != null) normalLight.enabled = true;

        // �v���C���[��~ & ���j���[�֎~
        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;

        // �v���C���[�̌��������ɌŒ�
        if (player != null) player.SetDirection(0);

        // NPC�o��
        if (sceneNpc != null)
        {
            sceneNpc.transform.position = npcSpawnPosition;
            sceneNpc.SetActive(true);
        }
        // �����I��Prefab���g���Ȃ炱������
        // if (npcPrefab != null) Instantiate(npcPrefab, npcSpawnPosition, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        Debug.Log("�L���������ꂽ: �w�悭�����ȁx");

        yield return new WaitForSeconds(3f);

        // �A�C�e������
        if (rewardItem != null)
        {
            InventoryManager.Instance.AddItem(rewardItem);
            Debug.Log($"�L��������A�C�e���w{rewardItem.itemName}�x���󂯎�����I");
        }

        yield return new WaitForSeconds(3f);

        Debug.Log("�L����: �w�ł͂܂�����c�x");
        if (sceneNpc != null) sceneNpc.SetActive(false);

        yield return new WaitForSeconds(2f);

        // �v���C���[���A & ���j���[����
        if (player != null) player.enabled = true;
        PauseMenu.blockMenu = false;
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