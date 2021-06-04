using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{
    //private
    private Wind wind;
    private ParticleSystem.VelocityOverLifetimeModule rainVel;
    private ParticleSystem.ShapeModule shape;
    private float rainSlantMag;
    private ParticleSystem rainPs;
    private Camera cam;
    private Vector2 rainPos;

    private void Awake()
    {
        wind = FindObjectOfType<Wind>();
        rainPs = GetComponent<ParticleSystem>();
        cam = Camera.main;
    }

    private void Start()
    {
        shape = rainPs.shape;
        rainVel = rainPs.velocityOverLifetime;
        rainVel.enabled = true;
        rainVel.x = -7.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (wind != null && wind.StormStarted)
        {
            var topCenterCamPos = cam.ViewportToWorldPoint(new Vector2(0.5f, 1));
            rainPos = new Vector2(topCenterCamPos.x, (topCenterCamPos.y + 20));
            shape.position = new Vector2(rainPos.x, rainPos.y);
            if (!rainPs.isPlaying)
                rainPs.Play();
            Debug.Log("RainPos is " + rainPos);
        }
    }
}
