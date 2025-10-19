using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public static bool isTitleActive = false; // ←★追加：タイトル表示中フラグ

    [Header("ボタン参照")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    [Header("カーソル設定")]
    public GameObject firstSelectedButton; // ← 「はじめから」ボタンなど

    [Header("フェード用UI（任意）")]
    [SerializeField] private CanvasGroup fadeCanvas; // 黒フェードに使うCanvasGroup

    private BootLoader boot;

    private void OnEnable()
    {
        isTitleActive = true; // ←★タイトル有効化

        // タイトルに戻った瞬間カーソル位置を「はじめから」に戻す
        if (EventSystem.current != null && firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    private void OnDisable()
    {
        isTitleActive = false; // ←★タイトル無効化
    }

    private void Start()
    {
        boot = FindObjectOfType<BootLoader>();

        // 念のため最初もカーソル合わせ
        if (EventSystem.current != null && startButton != null)
        {
            EventSystem.current.SetSelectedGameObject(startButton.gameObject);
        }
    }

    public void OnStartButton()
    {
        Debug.Log("[TitleManager] はじめから開始");

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
        // --- フェードアウト ---
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

        // --- シーンの初期化 ---
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name.StartsWith("Scene") || s.name == "GameOver")
            {
                foreach (var root in s.GetRootGameObjects())
                    root.SetActive(false);
            }
        }

        // --- ゲーム開始 ---
        boot.StartGame();

        // --- 少し待ってプレイヤーを初期化 ---
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
                Debug.Log("[TitleManager] プレイヤー位置をSpawnPointに初期化");
            }
        }

        // --- フェードイン ---
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

        // ★ここで確実に解除（最後に実行されるように）
        PauseMenu.blockMenu = false;
        Debug.Log("[TitleManager] メニューブロック解除（フェード完了後）");
    }

    public void OnContinueButton()
    {
        Debug.Log("[TitleManager] つづきから選択");
        if (SaveSlotUIManager.Instance != null)
            SaveSlotUIManager.Instance.OpenLoadPanel();
    }

    public void OnExitButton()
    {
        Debug.Log("[TitleManager] 終了 → タイトルへ戻る");
        boot.ReturnToTitle();
    }
}