using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // どこからでもアクセスするためのシングルトンインスタンス
    public static SoundManager Instance { get; private set; }

    [Header("■ Audio Mixer 設定")]
    [SerializeField] private AudioMixer audioMixer;
    private const string BGM_VOLUME_PARAM = "BGMVolume";
    private const string SE_VOLUME_PARAM = "SEVolume";

    // PlayerPrefsで使用するキー
    private const string BGM_KEY = "BGM_Volume";
    private const string SE_KEY = "SE_Volume";

    [Header("■ Audio Source 設定")]
    [SerializeField] private AudioSource bgmSource; // BGM再生用 (OutputをBGM Mixerグループへ)
    [SerializeField] private AudioSource seSource;  // SE再生用 (OutputをSE Mixerグループへ)

    private void Awake()
    {
        // シングルトンの初期化（永続化はBootstrapに任せる）
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 二重生成防止
        }
    }

    void Start()
    {
        // ゲーム開始時、保存された音量をロードする
        LoadVolumeSettings();
    }


    // --- BGM 制御 (AudioClipを直接引数で受け取る) ---

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM() { bgmSource.Stop(); }


    // --- SE 制御 (AudioClipを直接引数で受け取る) ---

    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;
        // SEは重ねて鳴らすため PlayOneShot を使用
        seSource.PlayOneShot(clip);
    }

    // --- 全体音量制御 (メニュー画面のスライダー連携用) ---

    /// <summary>
    /// BGMの全体音量を設定し、PlayerPrefsに保存する (SliderのOnValueChangedに設定)
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        // スライダー値 (0-1) を対数dBスケールに変換
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat(BGM_VOLUME_PARAM, dB);

        // 設定を保存
        PlayerPrefs.SetFloat(BGM_KEY, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// SEの全体音量を設定し、PlayerPrefsに保存する (SliderのOnValueChangedに設定)
    /// </summary>
    public void SetSEVolume(float volume)
    {
        // スライダー値 (0-1) を対数dBスケールに変換
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat(SE_VOLUME_PARAM, dB);

        // 設定を保存
        PlayerPrefs.SetFloat(SE_KEY, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 保存された音量をロードし、ミキサーに反映させる
    /// </summary>
    private void LoadVolumeSettings()
    {
        // デフォルト値 (1.0 = 最大音量)
        float defaultVolume = 1.0f;

        // BGM音量をロードし、ミキサーにセット
        // PlayerPrefs.GetFloatは、キーが存在しない場合、デフォルト値を使用する
        float bgmVol = PlayerPrefs.GetFloat(BGM_KEY, defaultVolume);
        SetBGMVolume(bgmVol);

        // SE音量をロードし、ミキサーにセット
        float seVol = PlayerPrefs.GetFloat(SE_KEY, defaultVolume);
        SetSEVolume(seVol);

        // 注意: ロードした値をUIスライダーに反映させる処理は、
        //       スライダーを制御する OptionPanelManager のような別のスクリプトで行ってください。
    }
}