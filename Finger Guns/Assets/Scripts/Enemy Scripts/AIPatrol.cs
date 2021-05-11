using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPatrol : MonoBehaviour
{
    #region Variables

    //Public
    [SerializeField] float walkSpeed;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] BoxCollider2D bodyCollider;

    //Components
    private Rigidbody2D rb2d;

    //Private
    private bool turnAround;
    #endregion

    #region Monobehaviour Callbacks

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }

    void Start()
    {
        Patrolling = true;
    }

    void Update()
    {
        if(Patrolling)
        {
            Patrol();
        }

        CharacterAnimation();
    }

    void FixedUpdate()
    {
        if (Patrolling)
        {
            turnAround = !Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);
        }
    }    
    #endregion

    #region Private Methods
    void Patrol()
    {
        rb2d.velocity = new Vector2(-walkSpeed * Time.fixedDeltaTime, rb2d.velocity.y);
        if (turnAround || bodyCollider.IsTouchingLayers(groundLayer))
            Flip();
    }

    void Flip()
    { 
        walkSpeed *= -1f;
        groundCheck.localPosition = new Vector3(groundCheck.localPosition.x * -1f, 
            groundCheck.localPosition.y, groundCheck.localPosition.z);
        
        //Move collider to other side of character due to the character not rotating on turn
        bodyCollider.offset = new Vector2(bodyCollider.offset.x * -1, bodyCollider.offset.y);
    }

    void CharacterAnimation()
    {
        Anim.SetFloat("Movement", rb2d.velocity.x);
    }
    #endregion

    //Properties
    public bool Patrolling { get; set; }
    public Animator Anim { get; set; }
}
