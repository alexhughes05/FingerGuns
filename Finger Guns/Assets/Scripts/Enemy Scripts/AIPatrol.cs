using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AIPatrol : MonoBehaviour
{
    #region Variables

    //Public
    [Space]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.1f;
    [Space]
    [SerializeField] Collider2D bodyCollider;
    [HideInInspector]
    [SerializeField] float detectionRange;
    [HideInInspector]
    [SerializeField] bool patrolling;
    [HideInInspector]
    [SerializeField] bool walkToNearestEdge;
    [HideInInspector]
    [SerializeField] float turnAroundDistance;
    [HideInInspector]
    [SerializeField] float walkSpeed;

    //Components
    private AIShoot shootScript;
    private FingerGunMan playerScript;
    private Coroutine co;
    private Beegman beegmanScript;
    private ExplodeyOne explodeyScript;
    private DetectionCircle detectionScript;
    private Rigidbody2D rb2d;

    //Private
    private bool currentlyFlipping;
    private Collider2D[] colliders;
    private bool signalTurn;
    private float distanceTraveledSinceTurn;
    private float currentXPos;
    private float prevXPos;
    #endregion

    #region Monobehaviour Callbacks

    private void Awake()
    {
        shootScript = GetComponent<AIShoot>();
        playerScript = FindObjectOfType<FingerGunMan>();
        beegmanScript = GetComponent<Beegman>();
        explodeyScript = GetComponent<ExplodeyOne>();
        detectionScript = GetComponentInChildren<DetectionCircle>();
        rb2d = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    private void Start()
    {
        if (detectionScript != null)
            detectionScript.DetectionRadius = detectionRange;
        currentXPos = transform.position.x;
        prevXPos = currentXPos;

        colliders = GetComponentsInChildren<Collider2D>();

        doneInitializing = true;
    }

    void Update()
    {
        currentXPos = transform.position.x;
        if (!walkToNearestEdge)
        {
            distanceTraveledSinceTurn += Mathf.Abs(currentXPos - prevXPos);

            if (distanceTraveledSinceTurn >= turnAroundDistance)
                signalTurn = true;
        }

        if (Patrolling && !EnemyAttack) //Not in aggro range of enemy
        {
            if (currentXPos - prevXPos > 0)
                EnemyFacingRight = true;
            else if (currentXPos - prevXPos < 0)
                EnemyFacingRight = false;
            Anim.SetFloat("Movement", rb2d.velocity.x);
            Patrol();
        }
        if (!EnemyAttack)
        {
            if (beegmanScript)
            {
                if (co != null)
                    StopCoroutine(co);
                beegmanScript.StartAttackingPlayer = false;
            }
        }
        else if (EnemyAttack)
        {
            FacePlayer();

            if (name.ToLower().Contains("beegman"))
            {
                if (beegmanScript && beegmanScript.StartAttackingPlayer == false)
                    co = StartCoroutine(beegmanScript.HeadButtCharge());  //If enemy is beegman and in range. Charge the player
            }
            else if (name.ToLower().Contains("explodeyone"))
            {
                if (explodeyScript && explodeyScript.MoveTowardsPlayer == false)
                    explodeyScript.MoveTowardsPlayer = true;
            }
        }
        prevXPos = currentXPos;
    }

    void FixedUpdate()
    {
        signalTurn = !Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer); //better if put either when in a charge or patrolling
    }
    #endregion

    #region Private Methods
    private void Patrol()
    {
        rb2d.velocity = new Vector2(-walkSpeed * Time.fixedDeltaTime, rb2d.velocity.y);
        if (signalTurn && !currentlyFlipping)
        {
            signalTurn = false;
            currentlyFlipping = true;
            Flip();
        }
    }

    public void FacePlayer()
    {
        if ((PlayerOnRightOfEnemy() && !EnemyFacingRight) || (!PlayerOnRightOfEnemy() && EnemyFacingRight))
        {
            if (beegmanScript != null)
                beegmanScript.NeedToFlipChargePs = !beegmanScript.NeedToFlipChargePs;

            EnemyHasBeenFlipped = !EnemyHasBeenFlipped;
            EnemyFacingRight = !EnemyFacingRight;

            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
    }

    private IEnumerator ResetTurnAround()
    {
        yield return new WaitForSeconds(1);
        currentlyFlipping = false;
    }

    public bool PlayerOnRightOfEnemy()
    {
        if (playerScript && playerScript.gameObject.transform.position.x > gameObject.transform.position.x)
            return true;
        else
            return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 12) //If you collide with another enemy. Get all of it's colliders and ignore each one. This allows enemies to pass through eachother.
        {
            foreach (Collider2D c1 in collision.gameObject.GetComponent<AIPatrol>().Cols)
            {
                foreach (Collider2D c2 in Cols)
                {
                    Physics2D.IgnoreCollision(c1, c2);
                }
            }
        }
        if (collision.gameObject.layer == 6 && !EnemyAttack) //if enemy runs into a wall, turn around
        {
            Flip();
        }
    }

    public void Flip()
    {
        if (shootScript != null)
            shootScript.FirePoint.transform.localPosition = new Vector2(-shootScript.FirePoint.transform.localPosition.x, shootScript.FirePoint.transform.localPosition.y);
        if (beegmanScript != null)
            beegmanScript.NeedToFlipChargePs = !beegmanScript.NeedToFlipChargePs;
        distanceTraveledSinceTurn = 0;
        walkSpeed *= -1f;
        groundCheck.localPosition = new Vector3(groundCheck.localPosition.x * -1f, groundCheck.localPosition.y, groundCheck.localPosition.z);

        //Flip scale for beegman
        if (name.ToLower().Contains("beegman") || name.ToLower().Contains("explodeyone"))
        {
            if (!EnemyAttack)
            {
                Vector3 newScale = transform.localScale;
                newScale.x *= -1;
                transform.localScale = newScale;
            }
        }

        StartCoroutine(ResetTurnAround());
    }
    #endregion

    //Properties
    public bool EnemyAttack { get; set; }
    public bool Patrolling { get { return patrolling; } set { patrolling = value; } }
    public bool WalkToNearestEdge { get { return walkToNearestEdge; } set { walkToNearestEdge = value; } }
    public bool doneInitializing { get; set; }
    public Animator Anim { get; set; }
    public Collider2D[] Cols { get { return colliders; } set { colliders = value; } }
    public bool EnemyHasBeenFlipped { get; set; }
    public bool EnemyFacingRight { get; set; }
}
