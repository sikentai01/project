using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // どこからでもアクセスするためのシングルトンインスタンス
    public static SoundManager Instance { get; private set; }

    [Header("■ Audio Mixer 設定")]
    // Audio Mixerの参照と公開パラメータ名
    [SerializeField] private AudioMixer audioMixer;
    private const string BGM_VOLUME_PARAM = "BGMVolume";
    private const string SE_VOLUME_PARAM = "SEVolume";

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
    public void SetBGMVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat(BGM_VOLUME_PARAM, dB);
    }
    public void SetSEVolume(float volume)
    {
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat(SE_VOLUME_PARAM, dB);
    }
}