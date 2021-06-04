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
    [SerializeField] float windFadeInTime;
    [SerializeField] float windFadeOutTime;

    //Private
    private ParticleSystem.VelocityOverLifetimeModule rainVel;
    private ParticleSystem.ShapeModule shape;
    private float rainSlantMag;

    // Start is called before the first frame update
    void Start()
    {
        if (windFadeInTime == 0)
            windFadeInTime = 0.1f;
        if (windFadeOutTime == 0)
            windFadeOutTime = 0.1f;
        rainVel = rainPS.velocityOverLifetime;
        shape = rainPS.shape;
        rainVel.enabled = true;
        rainVel.x = -7.5f;
    }

    public void StartStorm()
    {
        StormStarted = true;
        //rainPS.Play();
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
            Debug.Log("executing next gust");
        }
    }

    private IEnumerator WaitBeforeNextGust()
    {
        Debug.Log("Gust delay started.");
        var timeBtwGusts = UnityEngine.Random.Range(minTimeBtwGusts, maxTimeBtwGusts);
        yield return new WaitForSeconds(timeBtwGusts);

        var gustLength = UnityEngine.Random.Range(minGustLength, maxGustLength);
        yield return StartCoroutine(WindSpeedFadeIn(UnityEngine.Random.Range(minWindForce, maxWindForce) * (Random.Range(0, 2) * 2 - 1), windFadeInTime));

        Debug.Log("Max magnitude reached.");
        yield return new WaitForSeconds(gustLength);
        yield return StartCoroutine(WindSpeedFadeOut(0, windFadeOutTime));
        Debug.Log("wind gust now done.");
    }

    private IEnumerator WindSpeedFadeIn(float targetWindSpeed, float time)
    {
        WindActive = true;
        Debug.Log("Wind starting and ramping up velocity now");
        var elapsedTime = 0.0f;
        var shapeStartingPos = shape.position;
        if (targetWindSpeed < 0)
        {
            rainSlantMag = Mathf.Clamp(targetWindSpeed * 4, -maxRainSlant, -7.5f);
            while (elapsedTime < time)
            {
                rainVel.x = Mathf.Lerp(0, rainSlantMag, (elapsedTime / time));
                shape.position = Vector3.Lerp(shapeStartingPos, new Vector3((rainSlantMag * -1) + 5f, shape.position.y, shape.position.z), (elapsedTime / time));
                currentWindForce = Mathf.Lerp(0, targetWindSpeed, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            rainSlantMag = Mathf.Clamp(targetWindSpeed * 4, -7.5f, maxRainSlant);
            while (elapsedTime < time)
            {
                rainVel.x = Mathf.Lerp(0, rainSlantMag, (elapsedTime / time));
                shape.position = Vector3.Lerp(shapeStartingPos, new Vector3((rainSlantMag * -1) - 12.5f, shape.position.y, shape.position.z), (elapsedTime / time));
                currentWindForce = Mathf.Lerp(0, targetWindSpeed, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
    private IEnumerator WindSpeedFadeOut(float targetWindSpeed, float time)
    {
        Debug.Log("Wind dying down now");
        var startingRainSlantMag = rainSlantMag;
        var shapeStartingPos = shape.position;
        var startingWindForce = currentWindForce;
        var elapsedTime = 0.0f;

        while (elapsedTime < time)
        {
            rainVel.x = Mathf.Lerp(startingRainSlantMag, -7.5f, (elapsedTime / time));
            shape.position = Vector3.Lerp(shapeStartingPos, new Vector3(0, shape.position.y, shape.position.z), (elapsedTime / time));
            currentWindForce = Mathf.Lerp(startingWindForce, targetWindSpeed, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Wind fully stopped.");
        WindActive = false;
    }

    //Property
    public bool WindActive { get; set; }
    public float currentWindForce { get; set; }
    public bool StormStarted { get; set; }
}
