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
    private FingerGunMan playerScript;
    private Coroutine co;
    private Beegman beegmanScript;
    private ExplodeyOne explodeyScript;
    private DetectionCircle detectionScript;
    private Rigidbody2D rb2d;

    //Private
    private Collider2D[] colliders;
    private bool currentlyFlipping;
    private bool signalTurn;
    private float distanceTraveledSinceTurn;
    private float currentXPos;
    private float prevXPos;
    #endregion

    #region Monobehaviour Callbacks

    private void Awake()
    {
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
        if (!walkToNearestEdge)
        {
            currentXPos = transform.position.x;
            distanceTraveledSinceTurn += Mathf.Abs(currentXPos - prevXPos);
            prevXPos = currentXPos;

            if (distanceTraveledSinceTurn >= turnAroundDistance)
                signalTurn = true;
        }

        if (Patrolling) //Not in aggro range of enemy
        {
            if (beegmanScript)
            {
                if (co != null)
                    StopCoroutine(co);
                beegmanScript.StartAttackingPlayer = false;
            }
            Anim.SetFloat("Movement", rb2d.velocity.x);
            Patrol();
        }
        else
        {
            FacePlayer();

            if (name.ToLower().Contains("beegman"))
            {
                if (beegmanScript && beegmanScript.StartAttackingPlayer == false)
                    co = StartCoroutine(beegmanScript.HeadButtCharge());  //If enemy is beegman and in range. Charge the player
            }
            else if (name.ToLower().Contains("explodeyone"))
            {
                Debug.Log("moving towards me");
                if (explodeyScript && explodeyScript.MoveTowardsPlayer == false)
                    explodeyScript.MoveTowardsPlayer = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (Patrolling)
            signalTurn = !Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);
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

        if ((PlayerOnRightOfEnemey() && !EnemyFacingRight()) || (!PlayerOnRightOfEnemey() && EnemyFacingRight()))
        {
            Flip();
        }
    }

    private IEnumerator ResetTurnAround()
    {
        yield return new WaitForSeconds(1);
        currentlyFlipping = false;
    }

    public bool EnemyFacingRight()
    {
        if (name.ToLower().Contains("ghostyboi"))
        {
            if (bodyCollider.offset.x > 0)
                return true;
            else
                return false;
        }
        else
        {
            if (transform.localScale.x < 0)
                return true;
            else
                return false;
        }
    }

    public bool PlayerOnRightOfEnemey()
    {
        if (playerScript.gameObject.transform.position.x > transform.position.x)
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
    }

    public void Flip()
    {
        distanceTraveledSinceTurn = 0;
        walkSpeed *= -1f;
        groundCheck.localPosition = new Vector3(groundCheck.localPosition.x * -1f, groundCheck.localPosition.y, groundCheck.localPosition.z);

        //Move collider to other side of character due to the character not rotating on turn
        if (name.ToLower().Contains("ghostyboi"))
            bodyCollider.offset = new Vector2(bodyCollider.offset.x * -1, bodyCollider.offset.y);

        //Flip scale for beegman
        if (name.ToLower().Contains("beegman") || name.ToLower().Contains("explodeyone"))
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }

        StartCoroutine(ResetTurnAround());
    }
    #endregion

    //Properties
    public bool Patrolling { get { return patrolling; } set { patrolling = value; } }
    public bool WalkToNearestEdge { get { return walkToNearestEdge; } set { walkToNearestEdge = value; } }
    public bool doneInitializing { get; set; }
    public Animator Anim { get; set; }
    public Collider2D[] Cols { get { return colliders; } set { colliders = value; } }
}
