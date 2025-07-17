using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Tilemap tilemap;

    private Camera cam;
    private float halfHeight;
    private float halfWidth;

    void Start()
    {
        cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPos = target.position;
            newPos.z = transform.position.z;

            if (tilemap != null)
            {
                Bounds bounds = tilemap.localBounds;

                newPos.x = Mathf.Clamp(newPos.x,
                                       bounds.min.x + halfWidth,
                                       bounds.max.x - halfWidth);
                newPos.y = Mathf.Clamp(newPos.y,
                                       bounds.min.y + halfHeight,
                                       bounds.max.y - halfHeight);
            }

            transform.position = newPos;
        }
    }
}
