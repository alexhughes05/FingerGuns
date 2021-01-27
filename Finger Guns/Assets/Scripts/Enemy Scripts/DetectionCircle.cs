using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionCircle : MonoBehaviour
{
    private AIPatrol patrolScript;

    void Start()
    {
        patrolScript = GetComponentInParent<AIPatrol>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            patrolScript.patrolling = false;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            patrolScript.patrolling = true;
    }
}