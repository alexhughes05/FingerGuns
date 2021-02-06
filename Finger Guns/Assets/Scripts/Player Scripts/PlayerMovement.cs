﻿using System.Collections;
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
        PerformMovement();
        Debug.Log(grounded);
        if (bladeHit)
        {
            StartCoroutine(WaitAndStand());
        }
        Animation();
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
        Debug.Log("Falling is " + falling);
        //Movement
        if (anim.GetBool("Slide") == false)
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
            }
            //Somersault
            else if (facingRight && !dontFlip && Input.GetKey(KeyCode.D))
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, somersaultForceY);
            }
            else if (!facingRight && !dontFlip && Input.GetKey(KeyCode.A))
            {
                rb2d.velocity = new Vector2(-rb2d.velocity.x, somersaultForceY);
            }
            //Backflip
            else if (facingRight && !dontFlip && Input.GetKey(KeyCode.A))
                rb2d.velocity = new Vector2(-backflipForceX, backflipForceY);
            else if (!facingRight && !dontFlip && Input.GetKey(KeyCode.D))
                rb2d.velocity = new Vector2(-backflipForceX, backflipForceY);
        }        
        //Short hops
        if (Input.GetButtonUp("Jump") && rb2d.velocity.y > 0 && dontFlip)
        {
            AllowFalling();
            rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y / 2);
        }

        //Slide
        if (slideInput && horizontalInput != 0 && grounded)
        {
            if (facingRight && horizontalInput > 0)
                rb2d.AddForce(new Vector2(slideForce, 0), ForceMode2D.Impulse);
            else if (!facingRight && horizontalInput < 0)
                rb2d.AddForce(new Vector2(-slideForce, 0), ForceMode2D.Impulse);
        }
    }

    void Flip()
    {        
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;

        firePoint.Rotate(0f, 180f, 0f);
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
                !facingRight && !dontFlip)
                anim.SetTrigger("Somersault");
            else if (facingRight && !dontFlip || 
                !facingRight && !dontFlip)
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