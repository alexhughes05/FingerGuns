using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeAndRocksEffect : MonoBehaviour
{
    //Components
    private StartEruption startEruption;

    //private
    private ParticleSystem smoke;
    private ParticleSystem rocks;
    private float smokeDuration;

    void Awake()
    {
        startEruption = FindObjectOfType<StartEruption>();
        GetParticles();
    }

    // Start is called before the first frame update
    private void Start()
    {
        RocksDelayAfterSmoke = 3;
        smokeDuration = (startEruption.totalEruptionLength - startEruption.rumbleTimeBeforeSmoke);
        RocksDuration = (smokeDuration - RocksDelayAfterSmoke);
    }

    private void GetParticles()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        if (particles[0].name.Equals("VolcanoSmoke"))
        {
            smoke = particles[0];
            rocks = particles[1];

            
        }
        else
        {
            rocks = particles[0];
            smoke = particles[1];
        }
    }

    public void StartSmokeEffect()
    {
        smoke.Stop(); // Cannot set duration whilst particle system is playing
        var main = smoke.main;
        main.duration = smokeDuration;
        smoke.Play();
    }

    public void StartRocksEffect()
    {
        rocks.Stop(); // Cannot set duration whilst particle system is playing
        var main = rocks.main;
        main.duration = RocksDuration;
        rocks.Play();
    }

    //Properties
    #region Properties
    public float RocksDuration { get; set; }
    public float RocksDelayAfterSmoke { get; set; }
    #endregion
}
