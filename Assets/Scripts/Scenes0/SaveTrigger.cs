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

    private Light2D normalLight;
    private Light2D restrictedLight;

    void Start()
    {
        player = FindFirstObjectByType<GridMovement>();

        if (player != null)
        {
            // Normal �� Yuki �{�̂̃R���|�[�l���g����擾
            normalLight = player.GetComponent<Light2D>();

            // Restricted �� Yuki �̎q�I�u�W�F�N�g����擾
            var childLights = player.GetComponentsInChildren<Light2D>(true);
            foreach (var l in childLights)
            {
                if (l.name == "RestrictedLight") // �� Unity��̖��O�ɍ��킹��
                {
                    restrictedLight = l;
                }
            }

            // �V�[��0�J�n���͋������C�g����ON��
            if (restrictedLight != null) restrictedLight.enabled = true;
            if (normalLight != null) normalLight.enabled = false;
        }
    }
    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Return))
        {
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
        Debug.Log("�Z�[�u���ׂ�");
        //  ���̃^�C�~���O�Ń��C�g�؂�ւ�
        if (restrictedLight != null) restrictedLight.enabled = false;
        if (normalLight != null) normalLight.enabled = true;

        if (player != null) player.enabled = false;
        PauseMenu.blockMenu = true;
        if (player != null) player.SetDirection(0);

        yield return new WaitForSeconds(1.5f);

        Debug.Log("�L���������ꂽ: �w�悭�����ȁx");

        yield return new WaitForSeconds(3f);

        if (rewardItem != null)
        {
            InventoryManager.Instance.AddItem(rewardItem);
            Debug.Log($"�L��������A�C�e���w{rewardItem.itemName}�x���󂯎�����I");
        }

        yield return new WaitForSeconds(3f);

        Debug.Log("�L����: �w�ł͂܂�����c�x");

        yield return new WaitForSeconds(2f);

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