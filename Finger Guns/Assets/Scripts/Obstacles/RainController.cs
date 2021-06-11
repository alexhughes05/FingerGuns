using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{
    //public
    [Range(10, 100)]
    [SerializeField] float maxRainSlant;

    //private
    private const float P = -1.816216f;
    private const float N = -1.6f;
    private Wind wind;
    private ParticleSystem.VelocityOverLifetimeModule rainVel;
    private ParticleSystem.ShapeModule shape;
    private ParticleSystem rainPs;
    private Camera cam;
    private Vector2 currentPosAbovePlayer;
    private Vector3 fadeInShapeStartingPos;
    private float fadeInShapeEndingXPos;
    private float currentMaxRainSlant;
    private Vector2 rainFadeInLerpedPos;
    private float lerpedDisplacement;

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
            currentPosAbovePlayer = new Vector2(topCenterCamPos.x + 10, (topCenterCamPos.y + 20));

            shape.position = new Vector2(currentPosAbovePlayer.x + lerpedDisplacement, currentPosAbovePlayer.y);
            if (!rainPs.isPlaying)
                rainPs.Play();
        }
    }
    public IEnumerator AdjustRainSlantFadeIn(float targetRainSlant, float time)
    {
        var elapsedTime = 0.0f;
        fadeInShapeStartingPos = shape.position;
        currentMaxRainSlant = targetRainSlant;

        if (targetRainSlant < 0)
        {
            fadeInShapeEndingXPos = P * (targetRainSlant + 7.5f) + fadeInShapeStartingPos.x;
            while (elapsedTime < time)
            {
                rainVel.x = Mathf.Lerp(0, targetRainSlant, (elapsedTime / time));
                rainFadeInLerpedPos = Vector3.Lerp(fadeInShapeStartingPos, new Vector3(fadeInShapeEndingXPos, shape.position.y, shape.position.z), (elapsedTime / time));
                lerpedDisplacement = rainFadeInLerpedPos.x - fadeInShapeStartingPos.x;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            fadeInShapeEndingXPos = N * (targetRainSlant + 7.5f) + fadeInShapeStartingPos.x;
            while (elapsedTime < time)
            {
                rainVel.x = Mathf.Lerp(0, targetRainSlant, (elapsedTime / time));
                rainFadeInLerpedPos = Vector3.Lerp(fadeInShapeStartingPos, new Vector3(fadeInShapeEndingXPos, shape.position.y, shape.position.z), (elapsedTime / time));
                lerpedDisplacement = rainFadeInLerpedPos.x - fadeInShapeStartingPos.x;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    public IEnumerator AdjustRainSlantFadeOut(float targetRainSlant, float time)
    {
        var elapsedTime = 0.0f;

        while (elapsedTime < time)
        {
            rainVel.x = Mathf.Lerp(currentMaxRainSlant, targetRainSlant, (elapsedTime / time));
            var rainFadeOutLerpedPos = Vector3.Lerp(rainFadeInLerpedPos, new Vector3(fadeInShapeStartingPos.x, shape.position.y, shape.position.z), (elapsedTime / time));
            lerpedDisplacement = rainFadeOutLerpedPos.x - fadeInShapeStartingPos.x;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    //Properties
    public float MaxRainSlant { get { return maxRainSlant; } private set { maxRainSlant = value; } }
}
