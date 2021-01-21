using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPatrol : MonoBehaviour
{
    #region Variables
    //Components
    private Rigidbody2D rb2d;
    [HideInInspector]
    public Animator anim;

    //Public
    public float walkSpeed;
    public bool patrolling;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;

    //Private
    private bool turnAround;
    private bool facingRight;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        patrolling = true;
    }

    void Update()
    {
        if(patrolling)
        {
            Patrol();
        }

        CharacterAnimation();
    }

    void FixedUpdate()
    {
        if (patrolling)
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

    void Flip()
    { 
        walkSpeed *= -1f;
        groundCheck.localPosition = new Vector3(groundCheck.localPosition.x * -1f, 
            groundCheck.localPosition.y, groundCheck.localPosition.z);
    }

    void CharacterAnimation()
    {
        anim.SetFloat("Movement", rb2d.velocity.x);
    }
    #endregion
}
