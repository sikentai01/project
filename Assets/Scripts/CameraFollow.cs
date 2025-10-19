using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // �Ǐ]����Ώہi�v���C���[�j

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
                Debug.Log("[CameraFollow] Player�擾����: " + target.name);
            }
            return;
        }

        // �V���v���ȒǏ]�i�J�����ʒu���v���C���[�ɍ��킹��j
        Vector3 newPos = target.position;
        newPos.z = transform.position.z; // �J������Z�͌Œ�
        transform.position = Vector3.Lerp(transform.position, newPos, smoothSpeed * Time.deltaTime);
    }
}