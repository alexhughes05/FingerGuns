using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TweenSet
{
    public LeanTweenType easeType = LeanTweenType.linear;
    public float duration = 0.0f;
    public float delay = 0.0f;
    public bool isRelative = true;
    public bool loop = false;
    public bool pingPong = false;
    [HideInInspector]
    public LTDescr tweenObject;

    public void copySettings(TweenSet set)
    {
        easeType = set.easeType;
        duration = set.duration;
        delay = set.delay;
        isRelative = set.isRelative;
        loop = set.loop;
        pingPong = set.pingPong;
    }
}
