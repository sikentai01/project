using UnityEngine;

public class DialogueAdvanceInput : MonoBehaviour
{
    [SerializeField] private DialogueCore core;
    private bool isActive = false;

    void Awake()
    {
        if (!core) core = GetComponentInChildren<DialogueCore>(true);
        if (!core) core = GetComponentInParent<DialogueCore>();
        if (core)
        {
            core.OnConversationEnded += _ => isActive = false;
        }
    }

    // 会話開始時に呼ぶ（ボタンやトリガーから）
    public void SetActive(bool active) => isActive = active;

    void Update()
    {
        if (!isActive || core == null) return;
        if (Input.GetKeyDown(KeyCode.Return))
        {
            core.NextPage();
        }
    }
}
