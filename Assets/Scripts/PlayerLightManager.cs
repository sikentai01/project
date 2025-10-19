using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;

public class PlayerLightManager : MonoBehaviour
{
    [Header("懐中電灯設定")]
    [SerializeField] private Slider lightSlider;
    [SerializeField] private Light2D[] playerLights; //  複数ライト対応！
    [SerializeField] private TextMeshProUGUI lightValueText;

    private const string PLAYER_LIGHT_KEY = "PlayerLight_Level";

    // スライダー段階ごとの明るさ倍率
    private readonly float[] intensityLevels = {0.25f, 0.5f, 0.75f, 1.0f, 1.25f, 1.5f,1.75f };

    void Start()
    {
        // スライダー設定
        lightSlider.minValue = 1;
        lightSlider.maxValue = 7;
        lightSlider.wholeNumbers = true;

        int savedLevel = PlayerPrefs.GetInt(PLAYER_LIGHT_KEY, 4);
        lightSlider.SetValueWithoutNotify(savedLevel);
        lightSlider.onValueChanged.AddListener(OnLightChanged);

        ApplyLight(savedLevel);
    }

    private void OnLightChanged(float level)
    {
        int intLevel = Mathf.RoundToInt(level);
        ApplyLight(intLevel);

        PlayerPrefs.SetInt(PLAYER_LIGHT_KEY, intLevel);
        PlayerPrefs.Save();
    }

    private void ApplyLight(int level)
    {
        int index = Mathf.Clamp(level - 1, 0, intensityLevels.Length - 1);
        float intensity = intensityLevels[index];

        // 複数のLight2Dすべてに反映
        foreach (var light in playerLights)
        {
            if (light != null)
                light.intensity = intensity;
        }

        if (lightValueText != null)
            lightValueText.text = $"{level} / 7";
    }
}
