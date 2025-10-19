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
        else
        {
            yield return new WaitForSeconds(0.3f);
        }

        // --- �V�[���̏����� ---
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name.StartsWith("Scene") || s.name == "GameOver")
            {
                foreach (var root in s.GetRootGameObjects())
                    root.SetActive(false);
            }
        }

        // --- �Q�[���J�n ---
        boot.StartGame();

        // --- �����҂��ăv���C���[�������� ---
        yield return new WaitForSeconds(0.8f);

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var move = player.GetComponent<GridMovement>();
            if (move != null)
                move.enabled = true;

            var spawn = GameObject.Find("SpawnPoint");
            if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
                move.SetDirection(0);
                Debug.Log("[TitleManager] �v���C���[�ʒu��SpawnPoint�ɏ�����");
            }
        }

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

        // �������Ŋm���ɉ����i�Ō�Ɏ��s�����悤�Ɂj
        PauseMenu.blockMenu = false;
        Debug.Log("[TitleManager] ���j���[�u���b�N�����i�t�F�[�h������j");
    }

    public void OnContinueButton()
    {
        Debug.Log("[TitleManager] �Â�����I��");
        if (SaveSlotUIManager.Instance != null)
            SaveSlotUIManager.Instance.OpenLoadPanel();
    }

    public void OnExitButton()
    {
        Debug.Log("[TitleManager] �I�� �� �^�C�g���֖߂�");
        boot.ReturnToTitle();
    }
}