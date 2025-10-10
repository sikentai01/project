using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class BrightnessManager : MonoBehaviour
{
    [Header("明るさ設定")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private TMPro.TextMeshProUGUI brightnessText; // 数値表示用（任意）

    private const string BRIGHTNESS_KEY = "Brightness_Level"; // Level保存

    void Start()
    {
        //  スライダー設定
        brightnessSlider.minValue = 1;
        brightnessSlider.maxValue = 7;
        brightnessSlider.wholeNumbers = true; // 整数段階に固定

        // セーブデータ読み込み（デフォルトは3 = 明るさ0.3）
        int savedLevel = PlayerPrefs.GetInt(BRIGHTNESS_KEY, 3);
        brightnessSlider.SetValueWithoutNotify(savedLevel);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);

        // 初期反映
        ApplyBrightness(savedLevel);
    }

    private void OnBrightnessChanged(float level)
    {
        int intLevel = Mathf.RoundToInt(level);
        ApplyBrightness(intLevel);

        // 保存
        PlayerPrefs.SetInt(BRIGHTNESS_KEY, intLevel);
        PlayerPrefs.Save();
    }

    private void ApplyBrightness(int level)
    {
        float brightness = level * 0.1f; //  実際の明るさに変換

        if (globalLight != null)
            globalLight.intensity = brightness;

        if (brightnessText != null)
            brightnessText.text = $"{level}/7"; // 任意で表示（UIに）
    }
}
