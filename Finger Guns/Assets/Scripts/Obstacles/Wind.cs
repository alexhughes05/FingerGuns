using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    //public
    [Space()]
    [Header("General")]
    [SerializeField] float durationOfStorm;
    [Space()]
    [Header("Rain")]
    [SerializeField] ParticleSystem rainPS;
    [SerializeField] float maxRainSlant;
    [Space()]
    [Header("Wind")]
    [SerializeField] float minWindForce;
    [SerializeField] float maxWindForce;
    [SerializeField] float minGustLength;
    [SerializeField] float maxGustLength;
    [SerializeField] float minTimeBtwGusts;
    [SerializeField] float maxTimeBtwGusts;

    //Private
    private ParticleSystem.VelocityOverLifetimeModule rainVel;
    private ParticleSystem.ShapeModule shape;
    private float rainSlantMag;

    // Start is called before the first frame update
    void Start()
    {
        rainVel = rainPS.velocityOverLifetime;
        shape = rainPS.shape;
        rainVel.enabled = true;
        rainVel.x = -7.5f;
    }

    public void StartStorm()
    {
        StormStarted = true;
        rainPS.Play();
        StartCoroutine(StartWindForDuration());
    }

    private IEnumerator StartWindForDuration()
    {
        Coroutine co = StartCoroutine(WindGustCycle());
        yield return new WaitForSeconds(durationOfStorm);
        StopCoroutine(co);
        StormStarted = false;
    }

    private IEnumerator WindGustCycle()
    {
        while (true)
        {
            yield return WaitBeforeNextGust();
        }
    }

    private IEnumerator WaitBeforeNextGust()
    {
        var timeBtwGusts = UnityEngine.Random.Range(minTimeBtwGusts, maxTimeBtwGusts);
        yield return new WaitForSeconds(timeBtwGusts);
        var gustLength = UnityEngine.Random.Range(minGustLength, maxGustLength);
        currentWindForce = UnityEngine.Random.Range(minWindForce, maxWindForce) * (Random.Range(0, 2) * 2 - 1);
        if (currentWindForce < 0)
        {
            rainSlantMag = Mathf.Clamp(currentWindForce * 4, -maxRainSlant, -7.5f);
            rainVel.x = rainSlantMag;
            shape.position = new Vector2((rainSlantMag * -1) + 5f, shape.position.y);
        }
        else
        {
            rainSlantMag = Mathf.Clamp(currentWindForce * 4, -7.5f, maxRainSlant);
            rainVel.x = rainSlantMag;
            shape.position = new Vector2((rainSlantMag * -1) - 12.5f, shape.position.y);
        }
        WindActive = true;
        yield return new WaitForSeconds(gustLength);
        WindActive = false;
        currentWindForce = 0;
        shape.position = new Vector2(0, shape.position.y);
        rainVel.x = -7.5f;
    }

    //Property
    public bool WindActive { get; set; }
    public float currentWindForce { get; set; }
    public bool StormStarted { get; set; }
}
