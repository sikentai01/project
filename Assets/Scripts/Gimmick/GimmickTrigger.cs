using UnityEngine;

public class GimmickTrigger : MonoBehaviour
{
    [Header("���̃g���K�[�����삷��M�~�b�NID")]
    public string gimmickID;

    [Header("�Ή�����M�~�b�N�{��")]
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