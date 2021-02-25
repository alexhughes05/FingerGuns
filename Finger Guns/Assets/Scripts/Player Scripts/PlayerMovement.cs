using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    //Components
    private Rigidbody2D rb2d;
    [HideInInspector] public Animator anim;
    private PlayerHealth playerHealth;
    [Header("Components")]
    [SerializeField] PhysicsMaterial2D frictionMaterial;
    [SerializeField] PhysicsMaterial2D noFrictionMaterial;
    [Space()]

    //Public Variables
    [Header("Controller")]
    [HideInInspector]
    public bool hitByLightning = false;
    [HideInInspector]
    public bool resetShooting = false;
    public TimeManager timeManager;
    public bool playerDead = false;
    public bool flipPlayer;
    public float AFKTimer = 10f;
    public bool facingRight = true;
    public bool bladeHit = false;
    [SerializeField] float knockBackStrength = 15f;
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
    [SerializeField] float maxFallSpeed = 20f;
    [Header("Slide")]
    public float slideForce = 12f;
    [SerializeField] float slideDuration = 1f;
    [Header("Dodging")]
    public float flipWaitTime = 0.5f;
    public float somersaultForceX = 20f;
    public float somersaultForceY = 12.5f;
    public float backflipForceX = 10f;
    public float backflipForceY = 15f;
    [Space()]
    [Header("Weapon")]
    public Transform firePoint;
    [Space()]
    [Header("SFX")]
    private FMOD.Studio.EventInstance instance;
    [FMODUnity.EventRef]
    public string footstepSounds;
    [SerializeField] float walkInterval = 0.25f;

    //Other Private Variables
    private bool bladeHitSignal = false;
    private bool interruptLeftFlip;
    private bool interruptRightFlip;
    private bool firstPass = true;
    private bool canMove = true;
    private bool standingUp = true;
    private bool canShoot = true;
    private bool wasGrounded;
    private bool grounded;
    private bool falling;
    private float hangCounter;
    private float jumpBufferCounter;
    private float currentWalkInterval = 0;
    private bool isCoroutineStarted;
    private bool walkingState;

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
        playerHealth = GetComponent<PlayerHealth>();

        grounded = true;
        currentAFKTime = AFKTimer;
        wasGrounded = grounded;
    }

    private void Update()
    {
        GetInput();
        PerformMovement();
        TakingDamage();
        ChangeMaterials();
        Animation();
        SFX();
    }

    private void OnDisable()
    {
        horizontalInput = 0;
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

            if ((Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.Space)) || (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Space))) //need to set flipping to true here b/c if you wait till veleocity > 0 it's not instant
            {
                StopMovement();
                DisableFalling();
                if (standingUp)
                    canShoot = true;
            }


            if (Input.GetKeyDown(KeyCode.A) && !grounded)
            {
                interruptRightFlip = true;
            }

            if (Input.GetKeyDown(KeyCode.D) && !grounded)
            {
                interruptLeftFlip = true;
            }


            //Enable slowmotion
            if (Input.GetKeyDown(KeyCode.E))
            {
                timeManager.DoSlowmotion();
            }
            //Jump
            jumpInput = Input.GetButtonDown("Jump");
            if (jumpInput)
                grounded = false;
            //Slide
            slideInput = Input.GetKeyDown(KeyCode.S);
            //Crouch
            crouchInput = Input.GetKey(KeyCode.S);
        }
    }

    private void PerformMovement()
    {
        if (!bladeHit && !bladeHitSignal)
            ConfigureShooting();

        //Ground Check
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);

        if (grounded && standingUp)
        {
            standingUp = false;
        }
        //Falling Check
        falling = (rb2d.velocity.y < 0) && (!ignoreFalling) && (!grounded) && (hangCounter <= 0) && (bladeHit == false);
        //Limit falling speed
        rb2d.velocity = new Vector2(rb2d.velocity.x, Mathf.Clamp(rb2d.velocity.y, -maxFallSpeed, maxFallSpeed));

        //Movement
        if (anim.GetBool("Slide") == false && canMove)
        {
            rb2d.velocity = new Vector2(horizontalInput * movementSpeed, rb2d.velocity.y);
        }

        //Flip Player
        if (flipPlayer)
            Flip();

        //Hang time
        if (grounded)
            hangCounter = hangTime;
        else
            hangCounter -= Time.deltaTime;
        //Jump Buffer
        if (jumpInput)
            jumpBufferCounter = jumpBufferLength;
        else
            jumpBufferCounter -= Time.deltaTime;

        //Jump
        if (jumpBufferCounter >= 0 && hangCounter > 0 && rb2d.velocity.y <= 0 && !bladeHitSignal)
        {
            //Regular jumpz
            if (!flipping)
            {
                AllowFalling();
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpForce);
                flipping = false;
            }
            //Somersault
            else if (facingRight && Input.GetKey(KeyCode.D) && flipping)
            {
                canShoot = false;
                rb2d.velocity = new Vector2(somersaultForceX, somersaultForceY);
                flipping = true;
            }
            else if (!facingRight && Input.GetKey(KeyCode.A) && flipping)
            {
                canShoot = false;
                rb2d.velocity = new Vector2(-somersaultForceX, somersaultForceY);
                flipping = true;;
            }
            //Backflip
            else if (facingRight && Input.GetKey(KeyCode.A) && flipping)
            {
                canShoot = false;
                rb2d.velocity = new Vector2(-backflipForceX, backflipForceY);
                flipping = true;
            }
            else if (!facingRight && Input.GetKey(KeyCode.D) && flipping)
            {
                canShoot = false;
                rb2d.velocity = new Vector2(backflipForceX, backflipForceY);
                flipping = true;
            }
        }
        //Short hops
        if (Input.GetButtonUp("Jump") && rb2d.velocity.y > 0 && !flipping && !bladeHitSignal)
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

        if (flipping && rb2d.velocity.x > 0 && interruptRightFlip)
        {
            canMove = true;
            flipping = true;
            interruptRightFlip = false;
        }

        if (flipping && rb2d.velocity.x < 0 && interruptLeftFlip)
        {
            canMove = true;
            flipping = true;
            interruptLeftFlip = false;
        }
    }

    public void ConfigureShooting()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("FingerGunMan_Rig|Backflip") || anim.GetCurrentAnimatorStateInfo(0).IsName("FingerGunMan_Rig|Somersault"))
        {
            resetShooting = false;
            canShoot = false;
        }
        else if (hitByLightning)
        {
            resetShooting = false;
            canShoot = false;
        }
        else
        {
            resetShooting = true;
            canShoot = true;
        }
    }

    void TakingDamage()
    {
        if (bladeHit)
        {
            if (firstPass)
            {
                bladeHitSignal = true;
                firstPass = false;
                Vector2 upOne = new Vector2(0, 1);
                bool rightHit = Physics2D.OverlapCircle((Vector2)transform.position + upOne, 0.1f);

                if (rightHit)
                {
                    if (facingRight)
                    {
                        anim.SetTrigger("Fall Back");
                    }
                    else
                    {
                        anim.SetTrigger("Fall Forward");
                    }
                }
            }
            canShoot = false;
            canMove = false;
            standingUp = false;

            playerHealth.ModifyHealth(-1);

            DisableFalling();
            anim.ResetTrigger("Falling");

            rb2d.velocity = new Vector2(-knockBackStrength, -10);
            StartCoroutine(WaitAndStand());
        }
    }

    void ChangeMaterials()
    {
        if (grounded)
            rb2d.sharedMaterial = frictionMaterial;
        else
            rb2d.sharedMaterial = noFrictionMaterial;
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
        if (canMove)
        {
            anim.SetFloat("Walking", Mathf.Abs(horizontalInput));
        }

        //Jump, Somersault, & Backflip
        if (jumpBufferCounter >= 0 && hangCounter > 0 && !bladeHitSignal)
        {
            jumpBufferCounter = -0.1f;
            hangCounter = 0;

            if (!flipping)
                anim.SetTrigger("Jump");
            else if (facingRight && Input.GetKey(KeyCode.D) && flipping ||
                !facingRight && Input.GetKey(KeyCode.A) && flipping)
            {
                anim.SetTrigger("Somersault");
            }
            else if (facingRight && Input.GetKey(KeyCode.A) && flipping ||
                !facingRight && Input.GetKey(KeyCode.D) && flipping)
            {
                anim.SetTrigger("Backflip");
            }
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
            if (!flipping)
            {
                wasGrounded = grounded;;
            }
        }

        //Slide
        if (slideInput && horizontalInput != 0 && grounded && isCoroutineStarted == false)
        {
            if (facingRight && horizontalInput > 0 || !facingRight && horizontalInput < 0)
            {
                anim.SetBool("Slide", true);
                StartCoroutine(WaitToStopSlide());
            }
        }

        //Crouch
        if (crouchInput && horizontalInput <= 0 && grounded)
            anim.SetBool("Crouch", true);
        else
            anim.SetBool("Crouch", false);

        if (flipping && rb2d.velocity.y < 0)
            wasGrounded = false;
        

        if (grounded && !wasGrounded && !bladeHitSignal)
        {
            wasGrounded = true;
            flipping = false;
            canMove = true;
        }
        //Take Damage - Shot

        //Take Damage - Lightning        
    }

    void SFX()
    {
        //Walking
        walkingState = anim.GetCurrentAnimatorStateInfo(2).IsName("FingerGunMan_Rig|Walk");

        if (walkingState && grounded)
        {
            if (currentWalkInterval <= 0)
            {
                instance = FMODUnity.RuntimeManager.CreateInstance(footstepSounds);
                instance.start();
                instance.release();
                currentWalkInterval = walkInterval;
            }
            else
            {
                currentWalkInterval -= Time.deltaTime;
            }
        }
        else
        {
            currentWalkInterval = walkInterval;
        }
    }

    void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;

        firePoint.Rotate(0f, 180f, 0f);
    }

    public bool CanShoot()
    {
        return canShoot;
    }

    public void InitializeHitVariables()
    {
        bladeHit = false;
        canShoot = false;
        canMove = false;
    }

    void AllowFalling()
    {
        ignoreFalling = false;
    }

    void DisableFalling()
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
            yield return new WaitForSeconds(1);
            if (facingRight)
                anim.SetTrigger("Stand Up");
            else
                anim.SetTrigger("StandUp_Forward");
            ConfigureShooting();
            canMove = true;
            standingUp = true;
            firstPass = true;
            AllowFalling();
            bladeHitSignal = false;
        }
    }

    public IEnumerator WaitAndAllowMovement()
    {
        canMove = false;
        canShoot = false;
        yield return new WaitForSeconds(1);
        ShootAfterLightning();
        flipping = false;
        canMove = true;
    }

    void ShootAfterLightning()
    {
        hitByLightning = false;
        resetShooting = true;
        canShoot = true;
    }

    public void StopMovement()
    {
        flipping = true;
        canMove = false;
    }


    #endregion
}