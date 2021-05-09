using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStorm : MonoBehaviour
{
    //Components
    private Wind wind;

    private void Awake()
    {
        wind = FindObjectOfType<Wind>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
            wind.StartStorm();
    }
}
