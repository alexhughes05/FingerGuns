using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables
    public Transform followTarget;
    public float smoothSpeed = 0.2f;
    public Vector3 offset;
    #endregion

    #region Monobehaviour Callbacks
    void FixedUpdate()
    {
        Vector3 targetPosition = followTarget.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, 
            smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
    #endregion
}
