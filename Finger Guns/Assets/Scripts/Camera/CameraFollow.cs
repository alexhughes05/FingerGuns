using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables
    //public
    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 0.125f;
    [SerializeField] Vector3 offset;

    //private
    private Vector3 velocity = Vector3.zero;

    #endregion
    public void LateUpdate()
    {
        var desiredPos = target.position + offset;
        var smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothSpeed);
        transform.position = smoothedPos;
    }
}
