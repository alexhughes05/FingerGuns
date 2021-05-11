using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericCollisionHandler2D : MonoBehaviour
{
    [TagSelector]
    public string triggerTag = "Player";
    [Header("Collider Trigger Events")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(triggerTag)){
            if (onTriggerEnter != null)
            {
                onTriggerEnter.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag(triggerTag))
        {
            if (onTriggerExit != null)
            {
                onTriggerExit.Invoke();
            }
        }
    }
}