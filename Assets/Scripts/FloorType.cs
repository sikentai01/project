using UnityEngine;

public class FloorType : MonoBehaviour
{
    public enum SurfaceType
    {
        Tile,
        Mat,
        Dirt
    }

    [Header("‚±‚Ì°‚Ìí—Ş")]
    public SurfaceType surfaceType = SurfaceType.Tile;
}
