using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    public Transform target; // 追いかける対象

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 newPos = target.position;
            newPos.z = transform.position.z; // Z軸だけはカメラの値を使う
            transform.position = newPos;
        }
    }
}