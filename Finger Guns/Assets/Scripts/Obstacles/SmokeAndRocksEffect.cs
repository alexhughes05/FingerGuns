using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeAndRocksEffect : MonoBehaviour
{
    //Components
    private Eruption eruption;

    //private
    private ParticleSystem smoke;
    private ParticleSystem rocks;
    private float smokeDuration;

    void Awake()
    {
        eruption = FindObjectOfType<Eruption>();
        GetParticles();
    }

    // Start is called before the first frame update
    private void Start()
    {
        RocksDelayAfterSmoke = 3;
        smokeDuration = (eruption.totalEruptionLength - eruption.rumbleTimeBeforeSmoke);
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

    public void StopSmokeEffect()
    {
        smoke.Stop();
    }

    public void StartRocksEffect()
    {
        rocks.Stop(); // Cannot set duration whilst particle system is playing
        var main = rocks.main;
        main.duration = RocksDuration;
        rocks.Play();
    }

    public void StopRocksEffect()
    {
        rocks.Stop();
    }

    //Properties
    #region Properties
    public float RocksDuration { get; set; }
    public float RocksDelayAfterSmoke { get; set; }
    #endregion
}
