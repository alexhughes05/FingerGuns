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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && !alreadyExecuted)
        {
            alreadyExecuted = true;
            wind.StopStorm();
        }
    }
}
