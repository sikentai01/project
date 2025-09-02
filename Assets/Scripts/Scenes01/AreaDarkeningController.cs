using UnityEngine;

public class AreaDarkeningController : MonoBehaviour
{
    public Material targetMaterial; // このスクリプトで制御するマテリアル

    // インスペクターで設定するBoxColliderの参照
    public BoxCollider areaCollider;

    void Start()
    {
        // BoxColliderの範囲から暗くする座標範囲を設定
        if (targetMaterial != null && areaCollider != null)
        {
            targetMaterial.SetVector("_MinPoint", areaCollider.bounds.min);
            targetMaterial.SetVector("_MaxPoint", areaCollider.bounds.max);
        }
        else
        {
            Debug.LogError("マテリアルまたはコライダーが設定されていません。");
        }
    }
}
