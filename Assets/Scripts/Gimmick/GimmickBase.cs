using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    [Header("���̃M�~�b�N�ŗL��ID�i�Z�[�u�p�j")]
    public string gimmickID;

    [Header("���݂̐i�s�i�K")]
    public int currentStage = 0;

    public virtual GimmickSaveData SaveProgress()
    {
        return new GimmickSaveData
        {
            gimmickID = this.gimmickID,
            stage = this.currentStage
        };
    }

    public virtual void LoadProgress(int stage)
    {
        this.currentStage = stage;
    }
}