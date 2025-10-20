using UnityEngine;

public abstract class GimmickBase : MonoBehaviour
{
    [Header("このギミック固有のID（セーブ用）")]
    public string gimmickID;

    [Header("現在の進行段階")]
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