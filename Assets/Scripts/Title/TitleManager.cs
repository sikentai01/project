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

    public void OnContinueButton()
    {
        Debug.Log("[TitleManager] つづきから選択");

        PauseMenu.blockMenu = false;

        if (SaveSlotUIManager.Instance != null)
        {
            //  非アクティブな場合でも確実に有効化
            if (!SaveSlotUIManager.Instance.gameObject.activeSelf)
            {
                SaveSlotUIManager.Instance.gameObject.SetActive(true);
                Debug.Log("[TitleManager] SaveSlotUIManager を再アクティブ化しました。");
            }

            SaveSlotUIManager.Instance.OpenLoadPanel();
            Debug.Log("[TitleManager] OpenLoadPanel 呼び出し完了。");
        }
        else
        {
            Debug.LogWarning("[TitleManager] SaveSlotUIManager.Instance が null です。");
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

        // --- ゲーム開始 ---
        boot.StartGame();

        yield return new WaitForSeconds(0.8f);

        // --- プレイヤーを取得して完全初期化 ---
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var move = player.GetComponent<GridMovement>();
            var anim = player.GetComponent<Animator>();

            if (move != null)
            {
                move.enabled = false; // まず止める
            }

            // スポーンポイント探す
            var spawn = GameObject.Find("SpawnPoint");
            if (spawn != null)
            {
                player.transform.position = spawn.transform.position;
                if (move != null)
                {
                    move.SetDirection(0);
                    Debug.Log("[TitleManager] プレイヤー位置をSpawnPointに初期化");
                }
            }

            // アニメーションをリセット
            if (anim != null)
            {
                anim.SetBool("Move_motion", false);
                anim.SetInteger("Direction", 0);
            }

            yield return new WaitForSeconds(0.3f);

            // 移動スクリプトを再有効化
            if (move != null)
            {
                move.enabled = true;
                Debug.Log("[TitleManager] プレイヤー移動再有効化");
            }
        }

        // --- フラグ／メニュー解除 ---
        PauseMenu.blockMenu = false;
        Debug.Log("[TitleManager] メニューブロック解除");

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
    }

    public void OnLoadSlotSelected(int slotNumber)
    {
        Debug.Log($"[TitleManager] スロット{slotNumber}をロード開始");

        var data = SaveSystem.LoadGame(slotNumber);
        if (data == null)
        {
            Debug.LogWarning("[TitleManager] ロード失敗：データなし");
            return;
        }

        StartCoroutine(LoadSavedGameRoutine(data));
    }

    private IEnumerator LoadSavedGameRoutine(SaveSystem.SaveData data)
    {
        // フェードアウト
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

        // --- シーン切り替え ---
        var boot = FindObjectOfType<BootLoader>();
        if (boot != null)
        {
            boot.SetSceneActive("Title", false);
            boot.SetSceneActive(data.sceneName, true);
            Scene scene = SceneManager.GetSceneByName(data.sceneName);
            SceneManager.SetActiveScene(scene);
        }

        yield return new WaitForSeconds(0.3f);

        // --- プレイヤー位置復元 ---
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
            Debug.Log($"[TitleManager] プレイヤー位置ロード: {data.playerPosition}");
        }

        // --- 各マネージャー復元 ---
        if (InventoryManager.Instance != null && data.inventoryData != null)
            InventoryManager.Instance.LoadData(data.inventoryData);

        if (DocumentManager.Instance != null && data.documentData != null)
            DocumentManager.Instance.LoadData(data.documentData);

        if (GameFlags.Instance != null && data.flagData != null)
            GameFlags.Instance.LoadFlags(data.flagData);

        // --- メニュー解除 ---
        PauseMenu.blockMenu = false;
        TitleManager.isTitleActive = false;

        // フェードイン
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

        Debug.Log("[TitleManager] ロード完了！");
    }

    public void OnExitButton()
    {
        Debug.Log("[TitleManager] 終了 → タイトルへ戻る");
        boot.ReturnToTitle();
    }
}