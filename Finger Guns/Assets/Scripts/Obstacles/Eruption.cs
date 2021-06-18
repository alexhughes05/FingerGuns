using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using System.Threading;

public class Eruption : MonoBehaviour
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

    private void Awake()
    {
        smokeAndRocksEffect = FindObjectOfType<SmokeAndRocksEffect>();
        debris = FindObjectOfType<FallingDebris>();
    }

    public void StartEruption()
    {
        StartCoroutine(StartShaking());
    }
    public void StopEruption()
    {
        shaker.StartFadeOut(fadeOutTime);
        shaker.UpdateShake();
        StopAllCoroutines();
        smokeAndRocksEffect.StopRocksEffect();
        smokeAndRocksEffect.StopSmokeEffect();
        debris.StopRainingDebris();
    }

    private IEnumerator StartShaking()
    {
        Debug.Log("start of start shaking.");
        shaker = CameraShaker.Instance.StartShake(magnitudeValue, roughnessValue, fadeInTime);
        yield return new WaitForSeconds(rumbleTimeBeforeSmoke);
        smokeAndRocksEffect.StartSmokeEffect();
        StartCoroutine(StartRockEffect());
        Debug.Log("end of startshaking.");
    }

    private IEnumerator StartRockEffect()
    {
        Debug.Log("start of startrockeffect.");
        yield return new WaitForSeconds(smokeAndRocksEffect.RocksDelayAfterSmoke);
        smokeAndRocksEffect.StartRocksEffect();
        StartCoroutine(StartRainingDebris());
        Debug.Log("end of startrockeffect.");
    }

    private IEnumerator StartRainingDebris()
    {
        Debug.Log("start of startRainingDebris");
        yield return new WaitForSeconds(DelayToStartRainingDebris);
        debris.StartRainingDebris();
        StartCoroutine(StopShaking());
        Debug.Log("end of startRainingDebris");
    }

    private IEnumerator StopShaking()
    {
        Debug.Log("start of stopShaking.");
        yield return new WaitForSeconds(smokeAndRocksEffect.RocksDuration - DelayToStartRainingDebris);
        shaker.StartFadeOut(fadeOutTime);
        shaker.UpdateShake();
        Debug.Log("end of stopShaking.");
    }

    //Properties
    public float DelayToStartRainingDebris { get; set; } = 3;
}
