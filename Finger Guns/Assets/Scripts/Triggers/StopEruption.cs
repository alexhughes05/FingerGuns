using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopEruption : MonoBehaviour
{
    //Components
    Eruption eruptionScript;

    //private
    private bool alreadyTriggered;

    private void Awake()
    {
        eruptionScript = FindObjectOfType<Eruption>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !alreadyTriggered)
        {
            alreadyTriggered = true;
            eruptionScript.StopEruption();
        }
    }
}
