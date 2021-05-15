using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beegman : MonoBehaviour
{
    //public
    [SerializeField] float chargeDistancePastPlayer;
    [SerializeField] float minTimeBtwCharge;
    [SerializeField] float maxTimeBtwCharge;

    //Components
    private FingerGunMan playerScript;
    private AIPatrol patrolScript;
    private Animator anim;

    //private
    private bool playerHit;
    private Vector2 targetPos;
    private float playerXPos;
    private float distanceBeyondPlayer = 5;
    private bool inCharge;
    private bool enemyFacingRight;

    void Awake()
    {
        anim = GetComponent<Animator>();
        patrolScript = GetComponent<AIPatrol>();
        playerScript = FindObjectOfType<FingerGunMan>();
    }

    void Update()
    {
        if (playerScript)
        {
            playerXPos = playerScript.gameObject.transform.position.x;
            targetPos = new Vector2(playerXPos + distanceBeyondPlayer, transform.position.y);
            if (inCharge)
            {
                //Debug.Log("Current enemy xPos is " + transform.position.x + ". Curent targetPosX is " + targetPos.x);
                var moveSpeed = playerScript.MaxSpeed + 5;
                var movementThisFrame = moveSpeed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, targetPos, movementThisFrame);
                anim.SetFloat("Movement", moveSpeed);
                if (Vector2.Distance(transform.position, targetPos) <= 0.1 || playerHit)
                {
                    anim.SetFloat("Movement", 0);
                    if (!playerHit)
                        patrolScript.Flip();
                    inCharge = false;
                    playerHit = false;
                    Debug.Log("no longer charging.");
                }
                Debug.Log("currently charging.");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10) //hits player
        {
            playerHit = true;
            Debug.Log("hit player");
        }
    }

    public IEnumerator HeadButtCharge()
    {
        StartAttackingPlayer = true;
        while (true)
        {
            anim.SetFloat("Movement", 0);
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

        DetermineEnemyFacingDirection();
        DeterminePlayerDirection();
        if (PlayerOnRightOfEnemy)
        {
            Debug.Log("player on right");
            if (!enemyFacingRight)
                patrolScript.Flip();
            distanceBeyondPlayer = chargeDistancePastPlayer;
        }
        else if (!PlayerOnRightOfEnemy)
        {
            Debug.Log("player not on right.");
            if (enemyFacingRight)
                patrolScript.Flip();
            distanceBeyondPlayer = -chargeDistancePastPlayer;
            Debug.Log("distance beyond player is " + distanceBeyondPlayer);
            Debug.Log("player xPos is " + playerXPos);
        }

        inCharge = true;
    }

    private void DeterminePlayerDirection()
    {
        if (playerScript.gameObject.transform.position.x > transform.position.x)
            PlayerOnRightOfEnemy = true;
        else
            PlayerOnRightOfEnemy = false;
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
    public bool PlayerOnRightOfEnemy { get; set; }
}
