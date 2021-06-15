using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class StartEruption : MonoBehaviour
{
    //public
    [Space()]
    [Header("Volcano Rumble Info")]
    [SerializeField] float magnitudeValue;
    [SerializeField] float roughnessValue;
    [SerializeField] float fadeInTime;
    [SerializeField] float fadeOutTime;
    [Space()]
    [Header("Time Info")]
    public float rumbleTimeBeforeSmoke;
    public float totalEruptionLength;

    //Components
    private SmokeAndRocksEffect smokeAndRocksEffect;
    private FallingDebris debris;
    private CameraShakeInstance shaker;
    private bool alreadyTriggered;

    //private
    private IEnumerator EntireCoroutine;

    private void Awake()
    {
        smokeAndRocksEffect = FindObjectOfType<SmokeAndRocksEffect>();
        debris = FindObjectOfType<FallingDebris>();
    }
    private void Start()
    {
        EntireCoroutine = StartShaking();   
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !alreadyTriggered)
        {
            alreadyTriggered = true;
            StartCoroutine(EntireCoroutine);
        }
    }

    private IEnumerator StartShaking()
    {
        shaker = CameraShaker.Instance.StartShake(magnitudeValue, roughnessValue, fadeInTime);
        yield return new WaitForSeconds(rumbleTimeBeforeSmoke);
        smokeAndRocksEffect.StartSmokeEffect();
        yield return StartCoroutine(StartRockEffect());
    }

    private IEnumerator StartRockEffect()
    {
        yield return new WaitForSeconds(smokeAndRocksEffect.RocksDelayAfterSmoke);
        smokeAndRocksEffect.StartRocksEffect();
        yield return StartCoroutine(StartRainingDebris());
    }

    private IEnumerator StartRainingDebris()
    {
        yield return new WaitForSeconds(DelayToStartRainingDebris);
        debris.StartRainingDebris();
        yield return StartCoroutine(StopShaking());
    }

    private IEnumerator StopShaking()
    {
        yield return new WaitForSeconds(smokeAndRocksEffect.RocksDuration - DelayToStartRainingDebris);
        shaker.StartFadeOut(fadeOutTime);
        shaker.UpdateShake();
    }
    public void StopEruption()
    {
        if (EntireCoroutine != null)
        {
            StopAllCoroutines();
            shaker.StartFadeOut(fadeOutTime);
            shaker.UpdateShake();
        }
    }

    //Properties
    public float DelayToStartRainingDebris { get; set; } = 3;
}
