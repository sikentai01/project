using UnityEngine;

public class FloorType : MonoBehaviour
{
    public enum SurfaceType
    {
        Tile,
        Mat,
        Dirt
    }

    [Header("���̏��̎��")]
    public SurfaceType surfaceType = SurfaceType.Tile;
}
