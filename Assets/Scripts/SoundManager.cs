using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    // �ǂ�����ł��A�N�Z�X���邽�߂̃V���O���g���C���X�^���X
    public static SoundManager Instance { get; private set; }

    [Header("�� Audio Mixer �ݒ�")]
    // Audio Mixer�̎Q�Ƃƌ��J�p�����[�^��
    [SerializeField] private AudioMixer audioMixer;
    private const string BGM_VOLUME_PARAM = "BGMVolume";
    private const string SE_VOLUME_PARAM = "SEVolume";

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