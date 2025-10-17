using UnityEngine;

public class FloorType : MonoBehaviour
{
    public enum SurfaceType
    {
        Tile,
        Mat,
        Dirt,
        Wood
    }

    [Header("���̏��̎��")]
    public SurfaceType surfaceType = SurfaceType.Tile;
}
