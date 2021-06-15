using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    #region Variables
    //public
    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 0.125f;
    [SerializeField] Vector3 offset;

    //Components
    private StopCameraMovement cameraMovementScript;
    private FingerGunMan player;

    //private
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        player = FindObjectOfType<FingerGunMan>();
        cameraMovementScript = FindObjectOfType<StopCameraMovement>();
    }

    #endregion
    private void LateUpdate()
    {
        if (cameraMovementScript == null)
            if (!player.PlayerDead)
                MoveCamera();
        else if (!player.PlayerDead && cameraMovementScript.StopCameraFollow)
            MoveCamera();
    }

    private void MoveCamera()
    {
        var desiredPos = target.position + offset;
        var smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothSpeed);
        transform.position = smoothedPos;
    }
}
