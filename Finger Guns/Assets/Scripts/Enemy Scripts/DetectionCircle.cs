using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionCircle : MonoBehaviour
{
    private AIPatrol patrolScript;

    private void Awake()
    {
        patrolScript = GetComponentInParent<AIPatrol>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            patrolScript.Patrolling = false;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            patrolScript.Patrolling = true;
    }
}