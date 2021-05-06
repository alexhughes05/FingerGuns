using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class ShakeBehavior : MonoBehaviour
{
    [SerializeField] float distanceAwayToTriggerShake;
    [SerializeField] float maxMagnitudeValue;
    [SerializeField] float maxRoughnessValue;
    [SerializeField] float fadeInTime;
    [SerializeField] float fadeOutTime;

    private float roughnessValue;
    private float magnitudeValue;
    private float prevDistanceFromWall;
    private float currentDistanceFromWall;

    //reference to pressure wall
    private PressureWall pressureWall;
    
    //reference to fingergunman
    private FingerGunMan player;

    //Position of the pressure wall
    private float pressureWallXPos;

    //Position of the player
    private float playerXPos;

    private void Awake()
    {
        player = FindObjectOfType<FingerGunMan>();
        pressureWall = FindObjectOfType<PressureWall>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (pressureWall != null)
        {
            roughnessValue = 0f;
            magnitudeValue = 0f;
            pressureWallXPos = pressureWall.transform.position.x;
            playerXPos = player.transform.position.x;
            currentDistanceFromWall = playerXPos - pressureWallXPos;
            prevDistanceFromWall = currentDistanceFromWall;
        }
    }

    //Update is called once per frame
    void Update()
    {
        if (player && pressureWall != null)
        {
            playerXPos = player.transform.position.x;
            pressureWallXPos = pressureWall.GetPressureWallXPos();
            currentDistanceFromWall = playerXPos - pressureWallXPos;

            if (currentDistanceFromWall <= 0)
            {
                magnitudeValue = maxMagnitudeValue;
                roughnessValue = maxRoughnessValue;
            }

            else if (currentDistanceFromWall - prevDistanceFromWall <= (-1 / pressureWall.speedOfWall))
                IncreaseShaking();
            else if (currentDistanceFromWall - prevDistanceFromWall >= (1 / pressureWall.speedOfWall))
                DecreaseShaking();

            if (currentDistanceFromWall <= distanceAwayToTriggerShake)
            {
                CameraShaker.Instance.ShakeOnce(magnitudeValue, roughnessValue, fadeInTime, fadeOutTime);
            }
            else if (currentDistanceFromWall > distanceAwayToTriggerShake)
            {
                magnitudeValue = 0f;
                roughnessValue = 0f;
            }
        }
    }
    private void IncreaseShaking()
    {
        magnitudeValue += (maxMagnitudeValue / (distanceAwayToTriggerShake * pressureWall.speedOfWall));
        roughnessValue += (maxRoughnessValue / (distanceAwayToTriggerShake * pressureWall.speedOfWall));
        prevDistanceFromWall = currentDistanceFromWall;
    }

    private void DecreaseShaking()
    {
        magnitudeValue -= (maxMagnitudeValue / (distanceAwayToTriggerShake * pressureWall.speedOfWall));
        roughnessValue -= (maxRoughnessValue / (distanceAwayToTriggerShake * pressureWall.speedOfWall));
        prevDistanceFromWall = currentDistanceFromWall;
    }
}
