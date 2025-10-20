using UnityEngine;

public class GimmickTrigger : MonoBehaviour
{
    [Header("このトリガーが操作するギミックID")]
    public string gimmickID;

    [Header("対応するギミック本体")]
    public GimmickBase targetGimmick;

    public bool IsPlayerNear { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            IsPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            IsPlayerNear = false;
    }

    public T GetGimmick<T>() where T : GimmickBase
    {
        return targetGimmick as T;
    }
}