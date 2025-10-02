using UnityEngine;
using UnityEngine.UI;

public class OptionPanelManager : MonoBehaviour
{
    [Header("UI 参照")]
    public Slider bgmSlider; // インスペクターでBGM Sliderを設定
    public Slider seSlider;  // インスペクターでSE Sliderを設定

    // SoundManagerと同じPlayerPrefsキーを使用
    private const string BGM_KEY = "BGM_Volume";
    private const string SE_KEY = "SE_Volume";

    // PauseMenuのOpenOptions()から呼び出される
    public void InitializeSliders()
    {
        // PlayerPrefsから保存された値をロード
        float defaultVolume = 1.0f;
        float bgmVol = PlayerPrefs.GetFloat(BGM_KEY, defaultVolume);
        float seVol = PlayerPrefs.GetFloat(SE_KEY, defaultVolume);

        // スライダーの位置をロードした値に合わせる
        if (bgmSlider != null)
        {
            bgmSlider.value = bgmVol;
        }
        if (seSlider != null)
        {
            seSlider.value = seVol;
        }

        // ★補足: スライダーの値がセットされたことで、
        //         On Value Changedイベントが自動で発火し、
        //         SoundManager.SetVolume()が呼ばれてミキサーに音量がセットされます。
    }
}