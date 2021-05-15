using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beegman : MonoBehaviour
{
    //public
    [SerializeField] float minTimeBtwCharge;
    [SerializeField] float maxTimeBtwCharge;

    //Components
    private FingerGunMan playerScript;
    private AIPatrol patrolScript;
    private Animator anim;

    //private
    private Vector2 targetPos;
    private float playerXPos;
    private float distanceBeyondPlayer = 5;
    private bool inCharge;
    private bool enemyFacingRight;
    private bool playerOnRight;

    void Awake()
    {
        anim = GetComponent<Animator>();
        patrolScript = GetComponent<AIPatrol>();
        playerScript = FindObjectOfType<FingerGunMan>();
    }

    void Update()
    {
        targetPos = new Vector2(playerXPos + distanceBeyondPlayer, transform.position.y);
        playerXPos = playerScript.gameObject.transform.position.x;
        Debug.Log(playerXPos = playerScript.gameObject.transform.position.x);
        if (inCharge)
        {
            //Debug.Log("Current enemy xPos is " + transform.position.x + ". Curent targetPosX is " + targetPos.x);
            var moveSpeed = playerScript.MaxSpeed + 5;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, movementThisFrame);
            anim.SetFloat("Movement", moveSpeed);
            if (Vector2.Distance(transform.position, targetPos) <= 0.1)
            {
                anim.SetFloat("Movement", 0);
                patrolScript.Flip();
                inCharge = false;
                Debug.Log("no longer charging.");
            }
            Debug.Log("currently charging.");
        }
    }

    public IEnumerator HeadButtCharge()
    {
        StartAttackingPlayer = true;
        while (true)
        {
            anim.SetFloat("Movement", 0);
            DetermineEnemyFacingDirection();
            DeterminePlayerDirection();
            yield return new WaitUntil(() => !inCharge);
            Debug.Log("can charge again starting timer.");
            yield return WaitForNextCharge();
        }
    }

    private IEnumerator WaitForNextCharge()
    {
        var chargeTime = UnityEngine.Random.Range(minTimeBtwCharge, maxTimeBtwCharge);
        yield return new WaitForSeconds(chargeTime);
        Debug.Log("charging now.");
        if (playerOnRight)
        {
            if (!enemyFacingRight)
                patrolScript.Flip();
            distanceBeyondPlayer *= -1;
        }
        else if (!playerOnRight)
        {
            if (enemyFacingRight)
                patrolScript.Flip();
            distanceBeyondPlayer *= -1;
        }
        inCharge = true;
    }

    private void DeterminePlayerDirection()
    {
        if (playerScript.gameObject.transform.position.x > transform.position.x)
            playerOnRight = true;
        else
            playerOnRight = false;
    }
    private void DetermineEnemyFacingDirection()
    {
        if (transform.localScale.x < 0)
            enemyFacingRight = true;
        else
            enemyFacingRight = false;
    }

    //Properties
    public bool StartAttackingPlayer { get; set; }
}
