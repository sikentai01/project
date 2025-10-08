using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro を使う場合

public class OptionPanelManager : MonoBehaviour
{
    [Header("UI 参照")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    [Header("数値表示用テキスト")]
    [SerializeField] private TextMeshProUGUI bgmValueText;
    [SerializeField] private TextMeshProUGUI seValueText;

    private const string BGM_KEY = "BGM_Level";
    private const string SE_KEY = "SE_Level";

    private void Start()
    {
        InitializeSliders();
    }

    public void InitializeSliders()
    {
        int bgmLevel = PlayerPrefs.GetInt(BGM_KEY, 4);
        int seLevel = PlayerPrefs.GetInt(SE_KEY, 4);

        bgmSlider.SetValueWithoutNotify(bgmLevel);
        seSlider.SetValueWithoutNotify(seLevel);

        SoundManager.Instance.SetBGMLevel(bgmLevel);
        SoundManager.Instance.SetSELevel(seLevel);

        // 数値更新
        UpdateBGMText(bgmLevel);
        UpdateSEText(seLevel);

        // イベント登録
        bgmSlider.onValueChanged.AddListener(OnBGMValueChanged);
        seSlider.onValueChanged.AddListener(OnSEValueChanged);
    }

    private void OnBGMValueChanged(float level)
    {
        int intLevel = Mathf.RoundToInt(level);
        SoundManager.Instance.SetBGMLevel(intLevel);
        UpdateBGMText(intLevel);
    }

    private void OnSEValueChanged(float level)
    {
        int intLevel = Mathf.RoundToInt(level);
        SoundManager.Instance.SetSELevel(intLevel);
        UpdateSEText(intLevel);
    }

    private void UpdateBGMText(int level)
    {
        bgmValueText.text = $"{level} / 8";
    }

    private void UpdateSEText(int level)
    {
        seValueText.text = $"{level} / 8";
    }
}
