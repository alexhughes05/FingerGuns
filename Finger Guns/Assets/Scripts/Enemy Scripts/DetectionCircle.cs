using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionCircle : MonoBehaviour
{
    //Components
    private AIPatrol patrolScript;
    private Beegman beegmanScript;

    private void Awake()
    {
        DetectionCol = GetComponent<CircleCollider2D>();
        patrolScript = GetComponentInParent<AIPatrol>();
        beegmanScript = GetComponentInParent<Beegman>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => patrolScript.doneInitializing);
        DetectionCol.radius = DetectionRadius;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            patrolScript.EnemyAttack = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (beegmanScript != null)
            {
                beegmanScript.ChargeRemainingTime -= Time.time - beegmanScript.ChargeEnteredTime;
                if (beegmanScript.ChargeRemainingTime < 0)
                    beegmanScript.ChargeRemainingTime = 0;
            }
            patrolScript.EnemyAttack = false;
        }

        if (patrolScript.EnemyHasBeenFlipped)
        {
            patrolScript.EnemyHasBeenFlipped = false;
            patrolScript.EnemyFacingRight = !patrolScript.EnemyFacingRight;
            Vector3 newScale = transform.parent.gameObject.transform.localScale;
            newScale.x *= -1;
            transform.parent.gameObject.transform.localScale = newScale;
        }
    }

    //Properties
    public float DetectionRadius { get; set; }
    public CircleCollider2D DetectionCol { get; set; }
}