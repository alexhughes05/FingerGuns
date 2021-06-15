using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopCameraMovement : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
            StopCameraFollow = true;
    }

    //Properties
    public bool StopCameraFollow { get; set; }
}
