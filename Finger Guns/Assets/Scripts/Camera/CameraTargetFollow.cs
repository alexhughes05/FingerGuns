using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetFollow : MonoBehaviour
{
    public Transform player;

    private void LateUpdate()
    {
        if(player)
            transform.position = player.position;
    }
}