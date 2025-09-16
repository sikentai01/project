using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �V�[�����܂����ŃA�N�Z�X���邽�߂̐ÓI�ȃC���X�^���X
    public static GameManager instance;

    // �Q�[�g���J������Ԃ��ǂ������L�^����t���O
    public bool isGateOpen = false;

    // �V���O���g���p�^�[��������
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �V�[�����؂�ւ���Ă��j������Ȃ��悤�ɂ���
        }
        else
        {
            Destroy(gameObject);
        }
    }
}