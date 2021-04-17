using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;
    private Vector3 velocity = Vector3.zero;

    public void LateUpdate()
    {
        var desiredPos = target.position + offset;
        var smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothSpeed);
        transform.position = smoothedPos;
    }
}
