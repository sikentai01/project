using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLightController : MonoBehaviour
{
    [Header("ライト設定")]
    [Tooltip("懐中電灯ライト（プレイヤー本体に付いてる Light2D）")]
    public Light2D normalLight;      // ← Player本体にアタッチされてるライト

    [Tooltip("制限ライト（プレイヤーの子オブジェクト内）")]
    public Light2D restrictedLight;  // ← Playerの子オブジェクトにあるライト

    [Header("アイテム判定")]
    [Tooltip("懐中電灯アイテムのID（InventoryManagerに登録されているitemID）")]
    public string flashlightItemID = "item_flashlight";

    private InventoryManager inventory;

    private void Start()
    {
        inventory = InventoryManager.Instance;
        UpdateLightState(); // 初期状態チェック
    }

    private void Update()
    {
        // 所持チェック（必要ならイベントに置き換え可）
        UpdateLightState();
    }

    /// <summary>
    /// 懐中電灯アイテムの所持状況に応じてライトを切り替える
    /// </summary>
    public void UpdateLightState()
    {
        if (inventory == null)
        {
            Debug.LogWarning("[PlayerLightController] InventoryManager が見つかりません");
            return;
        }

        bool hasFlashlight = inventory.HasItem(flashlightItemID);

        if (hasFlashlight)
        {
            //  懐中電灯所持 → 通常ライトON、制限ライトOFF
            SetNormalLight(true);
        }
        else
        {
            //  懐中電灯なし → 通常ライトOFF、制限ライトON
            SetNormalLight(false);
        }
    }

    /// <summary>
    /// 明示的にライトを切り替える（他スクリプトから呼び出し可）
    /// </summary>
    public void SetNormalLight(bool enabled)
    {
        if (normalLight != null) normalLight.enabled = enabled;
        if (restrictedLight != null) restrictedLight.enabled = !enabled;
    }

    /// <summary>
    /// 懐中電灯を強制ONにする
    /// </summary>
    public void ForceFlashlightOn()
    {
        SetNormalLight(true);
    }

    /// <summary>
    /// 懐中電灯を強制OFFにする
    /// </summary>
    public void ForceFlashlightOff()
    {
        SetNormalLight(false);
    }
}