using UnityEngine;

public class SceneBGMController : MonoBehaviour
{
    [Header("���̃V�[����p��BGM")]
    [SerializeField] private AudioClip sceneBGM;

    private bool isPlaying = false;

    private void OnEnable()
    {
        // BootLoader�ŃV�[�����L�������ꂽ���iGameObject��SetActive(true)�ɂȂ����j
        if (sceneBGM != null)
        {
            Debug.Log($"[SceneBGMController] �V�[���L���� �� BGM�Đ��J�n: {sceneBGM.name}");
            SoundManager.Instance?.StopBGM();
            SoundManager.Instance?.PlayBGM(sceneBGM);
            isPlaying = true;
        }
        else
        {
            Debug.LogWarning($"[SceneBGMController] BGM���ݒ�: {gameObject.scene.name}");
        }
    }

    private void OnDisable()
    {
        // BootLoader�ŃV�[������A�N�e�B�u�����ꂽ���iSetActive(false)�j
        if (isPlaying)
        {
            Debug.Log($"[SceneBGMController] �V�[�������� �� BGM��~: {gameObject.scene.name}");
            SoundManager.Instance?.StopBGM();
            isPlaying = false;
        }
    }
}
