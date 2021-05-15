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
    [SerializeField] BoxCollider2D bodyCollider;
    [HideInInspector]
    [SerializeField] float detectionRange;
    [HideInInspector]
    [SerializeField] bool patrolling;
    [HideInInspector]
    [SerializeField] bool onPlatform;
    [HideInInspector]
    [SerializeField] float turnAroundDistance;
    [HideInInspector]
    [SerializeField] float walkSpeed;

    //Components
    private Coroutine co;
    private Beegman beegmanScript;
    private DetectionCircle detectionScript;
    private Rigidbody2D rb2d;

    //Private
    private bool turnAround;
    private float distanceTraveledSinceTurn;
    private float currentXPos;
    private float prevXPos;
    #endregion

    #region Monobehaviour Callbacks

    private void Awake()
    {
        beegmanScript = GetComponent<Beegman>();
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
        doneInitializing = true;
    }

    void Update()
    {
        if (!onPlatform)
        {
            currentXPos = transform.position.x;
            distanceTraveledSinceTurn += Mathf.Abs(currentXPos - prevXPos);
            prevXPos = currentXPos;

            if (distanceTraveledSinceTurn >= turnAroundDistance)
                turnAround = true;
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
            if (name.ToLower().Contains("beegman"))
                if (beegmanScript && beegmanScript.StartAttackingPlayer == false)
                    co = StartCoroutine(beegmanScript.HeadButtCharge());  //If enemy is beegman and in range. Charge the player
        }
    }

    void FixedUpdate()
    {
        if (Patrolling && onPlatform)
        {
            turnAround = !Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);
        }
    }    
    #endregion

    #region Private Methods
    void Patrol()
    {
        rb2d.velocity = new Vector2(-walkSpeed * Time.fixedDeltaTime, rb2d.velocity.y);
        if (turnAround)
            Flip();
    }

    public void Flip()
    {
        turnAround = false;
        distanceTraveledSinceTurn = 0;
        walkSpeed *= -1f;
        groundCheck.localPosition = new Vector3(groundCheck.localPosition.x * -1f, 
            groundCheck.localPosition.y, groundCheck.localPosition.z);
        
        //Move collider to other side of character due to the character not rotating on turn
        bodyCollider.offset = new Vector2(bodyCollider.offset.x * -1, bodyCollider.offset.y);

        //Flip scale for beegman
        if (name.Contains("Beegman"))
        {
            Vector3 newScale = transform.localScale;
            newScale.x *= -1;
            transform.localScale = newScale;
        }
    }
    #endregion

    //Properties
    public bool Patrolling { get { return patrolling; } set { patrolling = value; } }
    public bool OnPlatform { get { return onPlatform; } set { patrolling = value; } }
    public bool doneInitializing { get; set; }
    public Animator Anim { get; set; }
    public BoxCollider2D BodyCollider { get { return bodyCollider; } }
}
