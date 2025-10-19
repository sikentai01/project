using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public static bool isTitleActive = false; // �����ǉ��F�^�C�g���\�����t���O

    [Header("�{�^���Q��")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    [Header("�J�[�\���ݒ�")]
    public GameObject firstSelectedButton; // �� �u�͂��߂���v�{�^���Ȃ�

    [Header("�t�F�[�h�pUI�i�C�Ӂj")]
    [SerializeField] private CanvasGroup fadeCanvas; // ���t�F�[�h�Ɏg��CanvasGroup

    private BootLoader boot;

    private void OnEnable()
    {
        isTitleActive = true; // �����^�C�g���L����

        // �^�C�g���ɖ߂����u�ԃJ�[�\���ʒu���u�͂��߂���v�ɖ߂�
        if (EventSystem.current != null && firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    private void OnDisable()
    {
        isTitleActive = false; // �����^�C�g��������
    }

    private void Start()
    {
        boot = FindObjectOfType<BootLoader>();

        // �O�̂��ߍŏ����J�[�\�����킹
        if (EventSystem.current != null && startButton != null)
        {
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }
    }

    public void OnContinueButton()
    {
        Debug.Log("[TitleManager] �Â�����I��");

        PauseMenu.blockMenu = false;

        if (SaveSlotUIManager.Instance != null)
        {
            //  ��A�N�e�B�u�ȏꍇ�ł��m���ɗL����
            if (!SaveSlotUIManager.Instance.gameObject.activeSelf)
            {
                SaveSlotUIManager.Instance.gameObject.SetActive(true);
                Debug.Log("[TitleManager] SaveSlotUIManager ���ăA�N�e�B�u�����܂����B");
            }

            SaveSlotUIManager.Instance.OpenLoadPanel();
            Debug.Log("[TitleManager] OpenLoadPanel �Ăяo�������B");
        }
        else
        {
            Debug.LogWarning("[TitleManager] SaveSlotUIManager.Instance �� null �ł��B");
        }
    }

    public void OnStartButton()
    {
        Debug.Log("[TitleManager] �͂��߂���J�n");

        PauseMenu.blockMenu = true;

        if (GameFlags.Instance != null)
            GameFlags.Instance.ClearAllFlags();
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.ClearAll();
        if (DocumentManager.Instance != null)
            DocumentManager.Instance.ClearAll();

        StartCoroutine(StartNewGameRoutine());
    }

    private IEnumerator StartNewGameRoutine()
    {
        // --- �t�F�[�h�A�E�g ---
        if (fadeCanvas != null)
        {
            fadeCanvas.gameObject.SetActive(true);
            fadeCanvas.alpha = 0f;
            for (float t = 0; t < 1f; t += Time.deltaTime * 2f)
            {
                fadeCanvas.alpha = t;
                yield return null;
            }
            fadeCanvas.alpha = 1f;
        }

        // --- �Q�[���J�n ---
        boot.StartGame();

        yield return new WaitForSeconds(0.8f);

        // --- �v���C���[���擾���Ċ��S������ ---
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var move = player.GetComponent<GridMovement>();
            var anim = player.GetComponent<Animator>();

            if (move != null)
            {
                move.enabled = false; // �܂��~�߂�
            }

            // �X�|�[���|�C���g�T��
            var spawn = GameObject.Find("SpawnPoint");
            if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
                if (move != null)
                {
                    move.SetDirection(0);
                    Debug.Log("[TitleManager] �v���C���[�ʒu��SpawnPoint�ɏ�����");
                }
            }

            // �A�j���[�V���������Z�b�g
            if (anim != null)
            {
                anim.SetBool("Move_motion", false);
                anim.SetInteger("Direction", 0);
            }

            yield return new WaitForSeconds(0.3f);

            // �ړ��X�N���v�g���ėL����
            if (move != null)
            {
                move.enabled = true;
                Debug.Log("[TitleManager] �v���C���[�ړ��ėL����");
            }
        }

        // --- �t���O�^���j���[���� ---
        PauseMenu.blockMenu = false;
        Debug.Log("[TitleManager] ���j���[�u���b�N����");

        // --- �t�F�[�h�C�� ---
        if (fadeCanvas != null)
        {
            for (float t = 1f; t > 0f; t -= Time.deltaTime * 2f)
            {
                fadeCanvas.alpha = t;
                yield return null;
            }
            fadeCanvas.alpha = 0f;
            fadeCanvas.gameObject.SetActive(false);
        }
    }

    public void OnLoadSlotSelected(int slotNumber)
    {
        Debug.Log($"[TitleManager] �X���b�g{slotNumber}�����[�h�J�n");

        var data = SaveSystem.LoadGame(slotNumber);
        if (data == null)
        {
            Debug.LogWarning("[TitleManager] ���[�h���s�F�f�[�^�Ȃ�");
            return;
        }

        StartCoroutine(LoadSavedGameRoutine(data));
    }

    private IEnumerator LoadSavedGameRoutine(SaveSystem.SaveData data)
    {
        // �t�F�[�h�A�E�g
        if (fadeCanvas != null)
        {
            fadeCanvas.gameObject.SetActive(true);
            fadeCanvas.alpha = 0;
            for (float t = 0; t < 1; t += Time.deltaTime * 2f)
            {
                fadeCanvas.alpha = t;
                yield return null;
            }
            fadeCanvas.alpha = 1;
        }

        // --- �V�[���؂�ւ� ---
        var boot = FindObjectOfType<BootLoader>();
        if (boot != null)
        {
            boot.SetSceneActive("Title", false);
            boot.SetSceneActive(data.sceneName, true);
            Scene scene = SceneManager.GetSceneByName(data.sceneName);
            SceneManager.SetActiveScene(scene);
        }

        yield return new WaitForSeconds(0.3f);

        // --- �v���C���[�ʒu���� ---
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = data.playerPosition;
            var move = player.GetComponent<GridMovement>();
            if (move != null)
            {
                move.SetDirection(data.playerDirection);
                move.enabled = true;
            }
            Debug.Log($"[TitleManager] �v���C���[�ʒu���[�h: {data.playerPosition}");
        }

        // --- �e�}�l�[�W���[���� ---
        if (InventoryManager.Instance != null && data.inventoryData != null)
            InventoryManager.Instance.LoadData(data.inventoryData);

        if (DocumentManager.Instance != null && data.documentData != null)
            DocumentManager.Instance.LoadData(data.documentData);

        if (GameFlags.Instance != null && data.flagData != null)
            GameFlags.Instance.LoadFlags(data.flagData);

        // --- ���j���[���� ---
        PauseMenu.blockMenu = false;
        TitleManager.isTitleActive = false;

        // �t�F�[�h�C��
        if (fadeCanvas != null)
        {
            for (float t = 1; t > 0; t -= Time.deltaTime * 2f)
            {
                fadeCanvas.alpha = t;
                yield return null;
            }
            fadeCanvas.alpha = 0;
            fadeCanvas.gameObject.SetActive(false);
        }

        Debug.Log("[TitleManager] ���[�h�����I");
    }

    public void OnExitButton()
    {
        Debug.Log("[TitleManager] �I�� �� �^�C�g���֖߂�");
        boot.ReturnToTitle();
    }
}