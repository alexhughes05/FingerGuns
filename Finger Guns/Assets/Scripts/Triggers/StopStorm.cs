using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopStorm : MonoBehaviour
{
    //Components
    private Wind wind;

    //private
    private bool alreadyExecuted;

    private void Awake()
    {
        wind = FindObjectOfType<Wind>();
    }
}
