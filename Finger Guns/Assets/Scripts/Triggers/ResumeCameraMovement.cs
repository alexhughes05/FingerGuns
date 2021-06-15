using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeCameraMovement : MonoBehaviour
{
    private StopCameraMovement cameraMovementScript;

    private void Awake()
    {
        cameraMovementScript = FindObjectOfType<StopCameraMovement>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
            if (cameraMovementScript != null)
                cameraMovementScript.StopCameraFollow = false;
    }
}
