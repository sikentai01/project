#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Bootstrap�V�[���ƍ�ƃV�[���𓯎��ɊJ���Ă���Ƃ��A
/// ���s�J�n���Ƀ_�~�[�i�ҏW���̃V�[���̓��e�j�������Ŕ�A�N�e�B�u�����āA
/// BootLoader��Additive�Ń��[�h����{���̃V�[���Əd�����Ȃ��悤�ɂ���B
/// ���s��~��� DebugSceneAutoRestore.cs �������ōăA�N�e�B�u������B
/// </summary>
[ExecuteAlways]
public class DebugSceneHelper : MonoBehaviour
{
    [Header("�q�G�����L�[��Ɉꏏ�ɊJ���Ă����ƃV�[������o�^")]
    [SerializeField] private string[] sceneNames = { "Scenes0", "Scenes01", "Scenes02" };

    private void Awake()
    {
        if (Application.isPlaying)
        {
            foreach (var name in sceneNames)
            {
                var dummy = GameObject.Find(name);
                if (dummy != null)
                {
                    dummy.SetActive(false);
                    Debug.Log($"[DebugSceneHelper] ���s�J�n: {name} ���ꎞ�I�ɔ�A�N�e�B�u�����܂����B");
                }
            }
        }
    }
}
#endif