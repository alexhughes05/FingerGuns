using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    //Components
    private Rigidbody2D rb2d;
    [HideInInspector]
    public Animator anim;

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
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public float hangTime = 0.2f;
    public float jumpBufferLength = 0.1f;
    [Header("Slide")]
    [SerializeField] float slideDuration = 1f;
    [Header("Dodging")]
    public float jumpForce = 5f;
    public float somersaultForceX = 3f;
    public float somersaultForceY = 3f;
    public float slideForce = 12f;
    public float backflipForceX = 3f;
    public float backflipForceY = 15f;
    [Space()]    

    [Header("Weapon")]
    public Transform firePoint;

    //Other Private Variables        
    private bool grounded;
    private bool wasGrounded;
    private bool falling;
    private float hangCounter;
    private float jumpBufferCounter;
    private bool isCoroutineStarted;
    private bool maxSlideTimeReached;

    private float horizontalInput;
    private bool jumpInput;
    private bool slideInput;
    private bool crouchInput;
    private bool somersaultInput;
    private bool ignoreFalling;
    private bool backflipInput;

    private float currentAFKTime;
    #endregion

    #region Monobehaviour Callbacks 
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        currentAFKTime = AFKTimer;
        wasGrounded = grounded;
    }

    private void Update()
    {
        GetInput();
        PerformMovement();
        Animation();
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
            //Jump
            jumpInput = Input.GetButtonDown("Jump") && rb2d.velocity.x == 0;
            //SomerSault
            somersaultInput = Input.GetKeyDown(KeyCode.Space) && rb2d.velocity.x != 0;
            //Slide
            slideInput = Input.GetKeyDown(KeyCode.S) && rb2d.velocity.x != 0;
            //Crouch
            crouchInput = Input.GetKey(KeyCode.S) && rb2d.velocity.x == 0;
            //Backflip
            backflipInput = Input.GetKeyDown(KeyCode.Space) && rb2d.velocity.x != 0;

            //Stop falling animation if doing somersault or backflip
            if (somersaultInput || backflipInput)
                DisableFalling();
        }
    }
    private void PerformMovement()
    {
        //Ground Check
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);
        //Falling Check
        falling = ((rb2d.velocity.y < 0) && (!ignoreFalling) && (!grounded));

        //Movement
        if (anim.GetBool("Slide") == false)
        rb2d.velocity = new Vector2(horizontalInput * movementSpeed, rb2d.velocity.y); //Might have to put Time.deltaTime
        //Flip Player
        if(flipPlayer)
        {
            Flip();
        }

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
        if(jumpBufferCounter >= 0 && hangCounter > 0 && rb2d.velocity.x == 0)
        {
            rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpBufferCounter = 0;
        }
        if(Input.GetButtonUp("Jump") && rb2d.velocity.y > 0 && rb2d.velocity.x == 0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y / 2);
        }
        //SomerSault
        if (somersaultInput && grounded)
        {
            if (facingRight && rb2d.velocity.x > 0)
                rb2d.AddForce(new Vector2(somersaultForceX, somersaultForceY), ForceMode2D.Impulse);
            else if (!facingRight && rb2d.velocity.x < 0)
                rb2d.AddForce(new Vector2(-somersaultForceX, somersaultForceY), ForceMode2D.Impulse);
        }
        //Backflip
        if (backflipInput && grounded)
        {
            if (facingRight && rb2d.velocity.x < 0)
                rb2d.AddForce(new Vector2(-backflipForceX, backflipForceY), ForceMode2D.Impulse);
            else if (!facingRight && rb2d.velocity.x > 0)
                rb2d.AddForce(new Vector2(backflipForceX, backflipForceY), ForceMode2D.Impulse);
        }

        //Slide
        if (slideInput && grounded)
        {
            if (facingRight && rb2d.velocity.x > 0)
                rb2d.AddForce(new Vector2(slideForce, 0), ForceMode2D.Impulse);
            else if (!facingRight && rb2d.velocity.x < 0)
                rb2d.AddForce(new Vector2(-slideForce, 0), ForceMode2D.Impulse);
        }
    }

    void Flip()
    {        
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;

        firePoint.Rotate(0f, 180f, 0f);
    }
    public float GetPlayerXPos()
    {
        var xPos = gameObject.transform.position.x;
        Debug.Log(xPos);
        return xPos;
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

        //Jump, Fall, & Land
        if (jumpInput && grounded && rb2d.velocity.x == 0)
            anim.SetTrigger("Jump");            
        if (falling)
        {
            anim.SetTrigger("Falling");
            wasGrounded = false;
        }
        if (grounded && !wasGrounded)
        {
            anim.SetTrigger("Landing");
            anim.ResetTrigger("Falling");
            wasGrounded = grounded;
        }

        //Slide
        if (slideInput && isCoroutineStarted == false)
        {
            if (facingRight && rb2d.velocity.x > 0 || !facingRight && rb2d.velocity.x < 0)
            {
                anim.SetBool("Slide", true);
                StartCoroutine(WaitToStopSlide());
            }
        }

        //Crouch
        if (crouchInput && grounded)
        {
            anim.SetBool("Crouch", true);
        }
        else
            anim.SetBool("Crouch", false);

        //Somersault
        if (somersaultInput && grounded)
        {
            if (facingRight && rb2d.velocity.x > 0 || !facingRight && rb2d.velocity.x < 0)
                anim.SetTrigger("Somersault");
        }

        //Backflip
        if (backflipInput && grounded)
        {
            if (facingRight && rb2d.velocity.x < 0 || !facingRight && rb2d.velocity.x > 0)
                anim.SetTrigger("Backflip");
        }


        //Allow Falling
        if (anim.GetCurrentAnimatorStateInfo(2).IsName("FingerGunMan_Rig|Somersault")) //Still an issue here. Falling animation cuts off animation
        {
            if (anim.GetCurrentAnimatorStateInfo(2).normalizedTime >= 1)
            {
                AllowFalling();
            }
        }
        else if (anim.GetCurrentAnimatorStateInfo(2).IsName("FingerGunMan_Rig|Backflip")) //Still an issue here. Falling animation cuts off animation
        {
            if (anim.GetCurrentAnimatorStateInfo(2).normalizedTime >= 1)
            {
                AllowFalling();
            }
        }
    }

    private IEnumerator WaitToStopSlide()
    {
        isCoroutineStarted = true;
        yield return new WaitForSeconds(slideDuration);
        anim.SetBool("Slide", false);
        isCoroutineStarted = false;
    }

    private void AllowFalling()
    {
        ignoreFalling = false;
    }

    private void DisableFalling()
    {
        ignoreFalling = true;
    }    
    #endregion
}