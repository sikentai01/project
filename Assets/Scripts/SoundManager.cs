using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // �ǂ�����ł��A�N�Z�X���邽�߂̃V���O���g���C���X�^���X
    public static SoundManager Instance { get; private set; }

    [Header("�� Audio Mixer �ݒ�")]
    [SerializeField] private AudioMixer audioMixer;
    private const string BGM_VOLUME_PARAM = "BGMVolume";
    private const string SE_VOLUME_PARAM = "SEVolume";

    // PlayerPrefs�Ŏg�p����L�[
    private const string BGM_KEY = "BGM_Volume";
    private const string SE_KEY = "SE_Volume";

    [Header("�� Audio Source �ݒ�")]
    [SerializeField] private AudioSource bgmSource; // BGM�Đ��p (Output��BGM Mixer�O���[�v��)
    [SerializeField] private AudioSource seSource;  // SE�Đ��p (Output��SE Mixer�O���[�v��)

    private void Awake()
    {
        // �V���O���g���̏������i�i������Bootstrap�ɔC����j
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // ��d�����h�~
        }
    }

    void Start()
    {
        // �Q�[���J�n���A�ۑ����ꂽ���ʂ����[�h����
        LoadVolumeSettings();
    }


    // --- BGM ���� (AudioClip�𒼐ڈ����Ŏ󂯎��) ---

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM() { bgmSource.Stop(); }


    // --- SE ���� (AudioClip�𒼐ڈ����Ŏ󂯎��) ---

    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;
        // SE�͏d�˂Ė炷���� PlayOneShot ���g�p
        seSource.PlayOneShot(clip);
    }

    // --- �S�̉��ʐ��� (���j���[��ʂ̃X���C�_�[�A�g�p) ---

    /// <summary>
    /// BGM�̑S�̉��ʂ�ݒ肵�APlayerPrefs�ɕۑ����� (Slider��OnValueChanged�ɐݒ�)
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        // �X���C�_�[�l (0-1) ��ΐ�dB�X�P�[���ɕϊ�
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat(BGM_VOLUME_PARAM, dB);

        // �ݒ��ۑ�
        PlayerPrefs.SetFloat(BGM_KEY, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// SE�̑S�̉��ʂ�ݒ肵�APlayerPrefs�ɕۑ����� (Slider��OnValueChanged�ɐݒ�)
    /// </summary>
    public void SetSEVolume(float volume)
    {
        // �X���C�_�[�l (0-1) ��ΐ�dB�X�P�[���ɕϊ�
        float dB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;
        audioMixer.SetFloat(SE_VOLUME_PARAM, dB);

        // �ݒ��ۑ�
        PlayerPrefs.SetFloat(SE_KEY, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �ۑ����ꂽ���ʂ����[�h���A�~�L�T�[�ɔ��f������
    /// </summary>
    private void LoadVolumeSettings()
    {
        // �f�t�H���g�l (1.0 = �ő剹��)
        float defaultVolume = 1.0f;

        // BGM���ʂ����[�h���A�~�L�T�[�ɃZ�b�g
        // PlayerPrefs.GetFloat�́A�L�[�����݂��Ȃ��ꍇ�A�f�t�H���g�l���g�p����
        float bgmVol = PlayerPrefs.GetFloat(BGM_KEY, defaultVolume);
        SetBGMVolume(bgmVol);

        // SE���ʂ����[�h���A�~�L�T�[�ɃZ�b�g
        float seVol = PlayerPrefs.GetFloat(SE_KEY, defaultVolume);
        SetSEVolume(seVol);

        // ����: ���[�h�����l��UI�X���C�_�[�ɔ��f�����鏈���́A
        //       �X���C�_�[�𐧌䂷�� OptionPanelManager �̂悤�ȕʂ̃X�N���v�g�ōs���Ă��������B
    }
}