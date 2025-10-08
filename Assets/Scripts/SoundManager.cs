using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header(" Audio Mixer 設定")]
    [SerializeField] private AudioMixer audioMixer;
    private const string BGM_VOLUME_PARAM = "BGMVolume";
    private const string SE_VOLUME_PARAM = "SEVolume";

    private const string BGM_KEY = "BGM_Level";
    private const string SE_KEY = "SE_Level";

    [Header(" Audio Source 設定")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadVolumeSettings();
    }

    // --- BGM（整数化対応） ---
    public void SetBGMLevel(float level)
    {
        int intLevel = Mathf.RoundToInt(level);
        float dB = ConvertLevelToDecibel(intLevel);
        audioMixer.SetFloat(BGM_VOLUME_PARAM, dB);

        PlayerPrefs.SetInt(BGM_KEY, intLevel);
        PlayerPrefs.Save();
    }

    // --- SE（整数化対応） ---
    public void SetSELevel(float level)
    {
        int intLevel = Mathf.RoundToInt(level);
        float dB = ConvertLevelToDecibel(intLevel);
        audioMixer.SetFloat(SE_VOLUME_PARAM, dB);

        PlayerPrefs.SetInt(SE_KEY, intLevel);
        PlayerPrefs.Save();
    }

    // --- 保存読み込み ---
    private void LoadVolumeSettings()
    {
        int bgmLevel = PlayerPrefs.GetInt(BGM_KEY, 4); // デフォルト=4（標準）
        int seLevel = PlayerPrefs.GetInt(SE_KEY, 4);

        SetBGMLevel(bgmLevel);
        SetSELevel(seLevel);
    }

    // --- 8段階をdBに変換 ---
    private float ConvertLevelToDecibel(int level)
    {
        float volume = (level / 8f) * 2f;

        // log(0)回避
        if (volume <= 0f)
            return -80f; // 無音扱い

        // 線形倍率  デシベル
        float dB = Mathf.Log10(volume) * 20f;
        return dB;
    }


    // --- 再生系 ---
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM() => bgmSource.Stop();

    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;
        seSource.PlayOneShot(clip);
    }
}
