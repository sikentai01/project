using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // 追従する対象（プレイヤー）

    private Camera cam;
    private float smoothSpeed = 10f;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("[CameraFollow] Player取得成功: " + target.name);
            }
            return;
        }

        // シンプルな追従（カメラ位置をプレイヤーに合わせる）
        Vector3 newPos = target.position;
        newPos.z = transform.position.z; // カメラのZは固定
        transform.position = Vector3.Lerp(transform.position, newPos, smoothSpeed * Time.deltaTime);
    }
}