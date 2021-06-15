using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopEruption : MonoBehaviour
{
    //Components
    StartEruption startEruptionScript;

    //private
    private bool alreadyTriggered;

    private void Awake()
    {
        startEruptionScript = FindObjectOfType<StartEruption>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !alreadyTriggered)
        {
            alreadyTriggered = true;
            startEruptionScript.StopEruption();
        }
    }
}
