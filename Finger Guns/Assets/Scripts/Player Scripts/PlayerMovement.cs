using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    //Components
    private Rigidbody2D rb2d;
    [HideInInspector] public Animator anim;

    //Public Variables
    [Header("Controller")]
    public bool playerDead = false;
    public bool flipPlayer;
    public float AFKTimer = 10f;
    public bool facingRight = true;
    [Space()]
    [Header("Movement")]
    public float doubleTapWindow = 0.5f;
    public float movementSpeed = 100f;
    [Header("Jump")]
    public float jumpForce = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public float hangTime = 0.2f;
    public float jumpBufferLength = 0.1f;
    //[SerializeField] float maxFallSpeed = 20f; 
    [Header("Slide")]
    public float slideForce = 12f;
    [SerializeField] float slideDuration = 1f;
    [Header("Dodging")]
    public float somersaultForceX = 20f;
    public float somersaultForceY = 12.5f;
    public float backflipForceX = 10f;
    public float backflipForceY = 15f;
    [Space()]    
    [Header("Weapon")]
    public Transform firePoint;

    //Other Private Variables
    private bool bladeHit = false;
    private bool dontFlip = true;
    private bool wasGrounded;
    private bool grounded;
    private bool falling;
    private float hangCounter;
    private float jumpBufferCounter;
    private bool isCoroutineStarted;

    private float horizontalInput;
    private bool jumpInput;
    private bool slideInput;
    private bool crouchInput;
    private bool ignoreFalling;
    private bool sliding;
    private bool flipping;

    private float currentAFKTime;
    #endregion

    #region Monobehaviour Callbacks 

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        grounded = true;
        currentAFKTime = AFKTimer;
        wasGrounded = grounded;
    }

    private void Update()
    {
        GetInput();                
        Animation();
    }

    private void FixedUpdate()
    {
        PerformMovement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Blade"))
        {
            bladeHit = true;
            DisableFalling();
            anim.SetTrigger("Fall Back");
        }
    }
    #endregion

    #region Private Methods
    private void GetInput()
    {
        //If player is not dead, accept input
        if (!playerDead)
        {
            //Movement
            horizontalInput = Input.GetAxis("Horizontal");
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
            {
                dontFlip = true;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                dontFlip = false;
                DisableFalling();
            }
            //Jump
            jumpInput = Input.GetButtonDown("Jump");           
            //Slide
            slideInput = Input.GetKeyDown(KeyCode.S);
            //Crouch
            crouchInput = Input.GetKey(KeyCode.S);
        }
    }

    private void PerformMovement()
    {
        //Ground Check
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);
        //Falling Check
        falling = (rb2d.velocity.y < 0) && (!ignoreFalling) && (!grounded) && (hangCounter <= 0) && (bladeHit == false);
        //Limit falling speed
        //rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -maxFallSpeed, maxFallSpeed));
        //Debug.Log("Falling is " + falling);

        //Movement
        if (anim.GetBool("Slide") == false && !flipping)
            rb2d.velocity = new Vector2(horizontalInput * movementSpeed, rb2d.velocity.y);
        
        //Flip Player
        if(flipPlayer)
            Flip();

        //Hang time
        if (grounded)
            hangCounter = hangTime;
        else
            hangCounter -= Time.deltaTime;
        //Jump Buffer
        if(jumpInput)
            jumpBufferCounter = jumpBufferLength;
        else
            jumpBufferCounter -= Time.deltaTime;

        //Jump
        if(jumpBufferCounter >= 0 && hangCounter > 0 && rb2d.velocity.y <= 0)
        {
            //Regular jumpz
            if (dontFlip)
            {
                AllowFalling();
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
                flipping = false;
            }
            //Somersault
            else if (facingRight && !dontFlip && Input.GetKey(KeyCode.D) && !flipping)
            {
                rb2d.velocity = new Vector2(somersaultForceX, somersaultForceY);
                flipping = true;
                StartCoroutine(AllowMovement());
            }
            else if (!facingRight && !dontFlip && Input.GetKey(KeyCode.A) && !flipping)
            {                
                rb2d.velocity = new Vector2(-somersaultForceX, somersaultForceY);
                flipping = true;
                StartCoroutine(AllowMovement());
            }
            //Backflip
            else if (facingRight && !dontFlip && Input.GetKey(KeyCode.A) && !flipping)
            {
                rb2d.velocity = new Vector2(-backflipForceX, backflipForceY);
                flipping = true;
                StartCoroutine(AllowMovement());
            }
            else if (!facingRight && !dontFlip && Input.GetKey(KeyCode.D) && !flipping)
            {
                rb2d.velocity = new Vector2(backflipForceX, backflipForceY);
                flipping = true;
                StartCoroutine(AllowMovement());
            }
        }        
        //Short hops
        if (Input.GetButtonUp("Jump") && rb2d.velocity.y > 0 && dontFlip)
        {
            AllowFalling();
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y / 2);
        }

        //Slide
        if (slideInput && horizontalInput != 0 && grounded && !sliding)
        {
            sliding = true;

            if (facingRight && horizontalInput > 0)
                rb2d.AddForce(new Vector2(slideForce, 0), ForceMode2D.Impulse);
            else if (!facingRight && horizontalInput < 0)
                rb2d.AddForce(new Vector2(-slideForce, 0), ForceMode2D.Impulse);
        }

        //Taking Damage
        //Debug.Log(grounded);
        if (bladeHit)
        {
            StartCoroutine(WaitAndStand());
        }
    }    

    void Animation()
    {
        //Idle & AFK
        if (horizontalInput == 0)
        {
            if (currentAFKTime <= 0)
            {
                anim.SetTrigger("AFK");
                currentAFKTime = AFKTimer;
            }
            else
            {
                currentAFKTime -= Time.deltaTime;                
            }
        }

        //Movement
        anim.SetFloat("Walking", Mathf.Abs(horizontalInput));

        //Jump, Somersault, & Backflip
        if (jumpBufferCounter >= 0 && hangCounter > 0)
        {
            jumpBufferCounter = -0.1f;
            hangCounter = 0;

            if (dontFlip)
                anim.SetTrigger("Jump");
            else if (facingRight && horizontalInput > 0 && !dontFlip || 
                !facingRight && horizontalInput < 0 && !dontFlip)
                anim.SetTrigger("Somersault");
            else if (facingRight && horizontalInput < 0 && !dontFlip || 
                !facingRight && horizontalInput > 0 && !dontFlip)
                anim.SetTrigger("Backflip");
        }
        //Falling
        if (falling)
        {
            anim.SetTrigger("Falling");
            wasGrounded = false;
        }
        //Landing

        if (grounded && !wasGrounded && jumpBufferCounter < 0)
        {
            anim.SetTrigger("Landing");
            anim.ResetTrigger("Falling");
            wasGrounded = grounded;
        }

        //Slide
        if (slideInput && horizontalInput != 0  && grounded && isCoroutineStarted == false)
        {
            if (facingRight && horizontalInput > 0 || !facingRight && horizontalInput < 0)
            {
                anim.SetBool("Slide", true);
                StartCoroutine(WaitToStopSlide());
            }
        }

        //Crouch
        if (crouchInput && horizontalInput == 0 && grounded)
            anim.SetBool("Crouch", true);
        else
            anim.SetBool("Crouch", false);


    }

    void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;

        firePoint.Rotate(0f, 180f, 0f);
    }

    private void AllowFalling()
    {
        ignoreFalling = false;
    }

    private void DisableFalling()
    {
        ignoreFalling = true;
    }

    IEnumerator WaitToStopSlide()
    {
        isCoroutineStarted = true;
        yield return new WaitForSeconds(slideDuration);
        anim.SetBool("Slide", false);
        isCoroutineStarted = false;
        sliding = false;
    }

    IEnumerator WaitAndStand()
    {
        if (grounded)
        {
            bladeHit = false;
            Debug.Log("Player is grounded.");
            yield return new WaitForSeconds(1);
            Debug.Log("waited a second.");
            anim.SetTrigger("Stand Up");
            AllowFalling();
        }
    }

    IEnumerator AllowMovement()
    {
        float waitTime;
        waitTime = anim.GetCurrentAnimatorStateInfo(2).normalizedTime;
        yield return new WaitForSeconds(waitTime);
        flipping = false;
    }
    #endregion
}