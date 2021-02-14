using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager managerInstance { get; private set; }

    [FMODUnity.EventRef]
    public string music;
    FMOD.Studio.EventInstance instance;

    void Awake()
    {
        if(managerInstance == null)
        {
            managerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(music);
        instance.start();
    }

    public void GameStarted()
    {
        instance.setParameterByName("GameStarted", 1f);
    }

    public void TutorialShift()
    {
        instance.setParameterByName("TutorialShift", 1f);
    }
}