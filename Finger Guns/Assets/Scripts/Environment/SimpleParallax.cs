using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleParallax : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 lastCamPosition;
    private Vector3 camMoveVector;
    private float xParallax = 0.0f;
    private float yParallax = 0.0f;
    [SerializeField]
    private float xFollowAmount = 0.75f;
    [SerializeField]
    private float yFollowAmount = 0.25f;

    private void Start()
    {
        mainCam = Camera.main;
        lastCamPosition = mainCam.transform.position;
    }

    void Update()
    {
        camMoveVector = mainCam.transform.position - lastCamPosition;

        xParallax = camMoveVector.x * xFollowAmount;
        yParallax = camMoveVector.y * yFollowAmount;

        transform.Translate(new Vector3(xParallax, yParallax, 0.0f));

        lastCamPosition = mainCam.transform.position;
    }
}
