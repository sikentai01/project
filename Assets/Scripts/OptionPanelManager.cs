using UnityEngine;
using UnityEngine.UI;

public class OptionPanelManager : MonoBehaviour
{
    [Header("UI �Q��")]
    public Slider bgmSlider; // �C���X�y�N�^�[��BGM Slider��ݒ�
    public Slider seSlider;  // �C���X�y�N�^�[��SE Slider��ݒ�

    // SoundManager�Ɠ���PlayerPrefs�L�[���g�p
    private const string BGM_KEY = "BGM_Volume";
    private const string SE_KEY = "SE_Volume";

    // PauseMenu��OpenOptions()����Ăяo�����
    public void InitializeSliders()
    {
        // PlayerPrefs����ۑ����ꂽ�l�����[�h
        float defaultVolume = 1.0f;
        float bgmVol = PlayerPrefs.GetFloat(BGM_KEY, defaultVolume);
        float seVol = PlayerPrefs.GetFloat(SE_KEY, defaultVolume);

        // �X���C�_�[�̈ʒu�����[�h�����l�ɍ��킹��
        if (bgmSlider != null)
        {
            bgmSlider.value = bgmVol;
        }
        if (seSlider != null)
        {
            seSlider.value = seVol;
        }

        // ���⑫: �X���C�_�[�̒l���Z�b�g���ꂽ���ƂŁA
        //         On Value Changed�C�x���g�������Ŕ��΂��A
        //         SoundManager.SetVolume()���Ă΂�ă~�L�T�[�ɉ��ʂ��Z�b�g����܂��B
    }
}