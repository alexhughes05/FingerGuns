using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionCircle : MonoBehaviour
{
    private AIPatrol patrolScript;
    private CircleCollider2D col;

    private void Awake()
    {
        DetectionCol = GetComponent<CircleCollider2D>();
        patrolScript = GetComponentInParent<AIPatrol>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => patrolScript.doneInitializing);
        DetectionCol.radius = DetectionRadius;
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

    //Properties
    public float DetectionRadius { get; set; }
    public CircleCollider2D DetectionCol { get; set; }
}