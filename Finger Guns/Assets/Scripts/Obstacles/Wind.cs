using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    //public
    [Space()]
    [Header("General")]
    [SerializeField] private float durationOfStorm;
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

    //Components
    private RainController rainController;

    //private
    Coroutine co;

    private void Awake()
    {
        rainController = FindObjectOfType<RainController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (windFadeInTime == 0)
            windFadeInTime = 0.1f;
        if (windFadeOutTime == 0)
            windFadeOutTime = 0.1f;
    }

    public void StartStorm()
    {
        if (!StormStarted)
            StartCoroutine(StartWindForDuration());
        StormStarted = true;
    }

    public void StopStorm()
    {
        StopAllCoroutines();
        StormStarted = false;
        StartCoroutine(WaitAndStopWind());
    }

    private IEnumerator WaitAndStopWind()
    {
        yield return new WaitForSeconds(1.5f);
        currentWindForce = 0f;
        WindActive = false;
    }

    private IEnumerator StartWindForDuration()
    {
        co = StartCoroutine(WindGustCycle());
        yield return new WaitForSeconds(durationOfStorm);
        StopStorm();
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

        var randomWindSpeed = UnityEngine.Random.Range(minWindForce, maxWindForce) * (Random.Range(0, 2) * 2 - 1);
        float rainSlantBasedOnWind;
        
        if (randomWindSpeed < 0)
            rainSlantBasedOnWind = Mathf.Clamp(randomWindSpeed * (-rainController.MaxRainSlant / randomWindSpeed), -rainController.MaxRainSlant, -7.5f);
        else
            rainSlantBasedOnWind = Mathf.Clamp(randomWindSpeed * 4, -7.5f, rainController.MaxRainSlant);
        StartCoroutine(rainController.AdjustRainSlantFadeIn(rainSlantBasedOnWind, windFadeInTime));
        yield return StartCoroutine(LerpWindSpeed(0, randomWindSpeed, windFadeInTime, true));
        yield return new WaitForSeconds(gustLength);
        StartCoroutine(rainController.AdjustRainSlantFadeOut(-7.5f, windFadeOutTime));
        yield return StartCoroutine(LerpWindSpeed(currentWindForce, 0, windFadeOutTime, false));
    }

    private IEnumerator LerpWindSpeed(float startingWindSpeed, float targetWindSpeed, float time, bool isFadingIn)
    {
        if (isFadingIn) //wind gust starting and fading in
            WindActive = true;
        var elapsedTime = 0.0f;

        while (elapsedTime < time)
        {
            currentWindForce = Mathf.Lerp(startingWindSpeed, targetWindSpeed, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!isFadingIn) //wind gust ending and fading out
            WindActive = false;
    }

    //Property
    public bool WindActive { get; set; }
    public float currentWindForce { get; set; }
    public bool StormStarted { get; set; }
}
