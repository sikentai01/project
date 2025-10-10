using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class BrightnessManager : MonoBehaviour
{
    [Header("���邳�ݒ�")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Light2D globalLight;
    [SerializeField] private TMPro.TextMeshProUGUI brightnessText; // ���l�\���p�i�C�Ӂj

    private const string BRIGHTNESS_KEY = "Brightness_Level"; // Level�ۑ�

    void Start()
    {
        //  �X���C�_�[�ݒ�
        brightnessSlider.minValue = 1;
        brightnessSlider.maxValue = 7;
        brightnessSlider.wholeNumbers = true; // �����i�K�ɌŒ�

        // �Z�[�u�f�[�^�ǂݍ��݁i�f�t�H���g��3 = ���邳0.3�j
        int savedLevel = PlayerPrefs.GetInt(BRIGHTNESS_KEY, 3);
        brightnessSlider.SetValueWithoutNotify(savedLevel);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);

        // �������f
        ApplyBrightness(savedLevel);
    }

    private void OnBrightnessChanged(float level)
    {
        int intLevel = Mathf.RoundToInt(level);
        ApplyBrightness(intLevel);

        // �ۑ�
        PlayerPrefs.SetInt(BRIGHTNESS_KEY, intLevel);
        PlayerPrefs.Save();
    }

    private void ApplyBrightness(int level)
    {
        float brightness = level * 0.1f; //  ���ۂ̖��邳�ɕϊ�

        if (globalLight != null)
            globalLight.intensity = brightness;

        if (brightnessText != null)
            brightnessText.text = $"{level}/7"; // �C�ӂŕ\���iUI�Ɂj
    }
}
