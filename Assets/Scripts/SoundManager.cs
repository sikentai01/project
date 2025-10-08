using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header(" Audio Mixer �ݒ�")]
    [SerializeField] private AudioMixer audioMixer;
    private const string BGM_VOLUME_PARAM = "BGMVolume";
    private const string SE_VOLUME_PARAM = "SEVolume";

    private const string BGM_KEY = "BGM_Level";
    private const string SE_KEY = "SE_Level";

    [Header(" Audio Source �ݒ�")]
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

    // --- BGM�i�������Ή��j ---
    public void SetBGMLevel(float level)
    {
        int intLevel = Mathf.RoundToInt(level);
        float dB = ConvertLevelToDecibel(intLevel);
        audioMixer.SetFloat(BGM_VOLUME_PARAM, dB);

        PlayerPrefs.SetInt(BGM_KEY, intLevel);
        PlayerPrefs.Save();
    }

    // --- SE�i�������Ή��j ---
    public void SetSELevel(float level)
    {
        int intLevel = Mathf.RoundToInt(level);
        float dB = ConvertLevelToDecibel(intLevel);
        audioMixer.SetFloat(SE_VOLUME_PARAM, dB);

        PlayerPrefs.SetInt(SE_KEY, intLevel);
        PlayerPrefs.Save();
    }

    // --- �ۑ��ǂݍ��� ---
    private void LoadVolumeSettings()
    {
        int bgmLevel = PlayerPrefs.GetInt(BGM_KEY, 4); // �f�t�H���g=4�i�W���j
        int seLevel = PlayerPrefs.GetInt(SE_KEY, 4);

        SetBGMLevel(bgmLevel);
        SetSELevel(seLevel);
    }

    // --- 8�i�K��dB�ɕϊ� ---
    private float ConvertLevelToDecibel(int level)
    {
        float volume = (level / 8f) * 2f;

        // log(0)���
        if (volume <= 0f)
            return -80f; // ��������

        // ���`�{��  �f�V�x��
        float dB = Mathf.Log10(volume) * 20f;
        return dB;
    }


    // --- �Đ��n ---
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
