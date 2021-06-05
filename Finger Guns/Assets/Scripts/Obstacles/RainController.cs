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
    private Vector2 rainPos;
    private float currentMaxRainSlant;

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
        }
    }
    public IEnumerator AdjustRainSlantFadeIn(float targetRainSlant, float time)
    {
        var elapsedTime = 0.0f;
        var shapeStartingPos = shape.position;
        currentMaxRainSlant = targetRainSlant;

        if (targetRainSlant < 0)
        {
            while (elapsedTime < time)
            {
                Debug.Log("For rain controller, t is " + (elapsedTime / time));
                rainVel.x = Mathf.Lerp(0, targetRainSlant, (elapsedTime / time));
                shape.position = Vector3.Lerp(shapeStartingPos, new Vector3(P * (targetRainSlant - -7.5f) + shape.position.x, shape.position.y, shape.position.z), (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            while (elapsedTime < time)
            {
                Debug.Log("For rain controller, t is " + (elapsedTime / time));
                rainVel.x = Mathf.Lerp(0, targetRainSlant, (elapsedTime / time));
                shape.position = Vector3.Lerp(shapeStartingPos, new Vector3(N * (targetRainSlant - -7.5f) + shape.position.x, shape.position.y, shape.position.z), (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    public IEnumerator AdjustRainSlantFadeOut(float targetRainSlant, float time)
    {
        var elapsedTime = 0.0f;
        var shapeStartingPos = shape.position;

        while (elapsedTime < time)
        {
            rainVel.x = Mathf.Lerp(currentMaxRainSlant, targetRainSlant, (elapsedTime / time));
            shape.position = Vector3.Lerp(shapeStartingPos, new Vector3(0, shape.position.y, shape.position.z), (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    //Properties
    public float MaxRainSlant { get { return maxRainSlant; } private set { maxRainSlant = value; } }
}
