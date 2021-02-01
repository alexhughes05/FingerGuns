using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TweenSetFloat : TweenSet
{
    public float from = 0.0f;
    public float to = 0.0f;
    [HideInInspector]
    public float originalValue;
}
