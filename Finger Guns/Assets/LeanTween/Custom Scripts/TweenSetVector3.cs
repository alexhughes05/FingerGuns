using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TweenSetVector3 : TweenSet
{
    public Vector3 from = Vector3.zero;
    public Vector3 to = Vector3.zero;
    [HideInInspector]
    public Vector3 originalValue;
}
