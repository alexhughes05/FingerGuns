using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FingerGunMan : MonoBehaviour
{
    #region Variables

    //Public Variables
    [Space()]
    [Header("Physics Materials")]
    [SerializeField] PhysicsMaterial2D frictionMaterial;
    [SerializeField] PhysicsMaterial2D noFrictionMaterial;
    [Space()]
    [Header("Movement")]
    [SerializeField] float maxSpeed = 15f;
    [Space()]
    [Header("Jump")]
    [SerializeField] float jumpForce = 15;
    [SerializeField] float jumpDelay = 0.25f;
    [SerializeField] float coyoteTime = 0.2f;
    [Space()]
    [Header("Flips")]
    [SerializeField] float somersaultForceX = 1500f;
    [SerializeField] float somersaultForceY = 600f;
    [SerializeField] float backflipForceX = 400f;
    [SerializeField] float backflipForceY = 1000f;
    [Space()]
    [Header("Slide")]
    [SerializeField] float slideForce = 20f;
    [SerializeField] float slideDuration = 0.6f;
    [SerializeField] float timeBtwSlides = 0.2f;
    [Space()]
    [Header("Obstacle Collision")]
    [SerializeField] float knockbackStrength = 15f;
    [Space()]
    [Header("Gravity")]
    [SerializeField] float gravity = 4f;
    [SerializeField] float fallMultiplier = 2.5f;
    [Space()]
    [Header("Particles")]
    [SerializeField] ParticleSystem dust;
    [SerializeField] float dustEmissionRate = 15f;
    [SerializeField] ParticleSystem impactEffect;
    [Space()]
    [Header("Time")]
    [SerializeField] TimeManager timeManager;
    [SerializeField] float AFKTimer = 10f;
    [Space()]
    [Header("Weapon")]
    [SerializeField] Transform firePoint;
    [Space()]
    [Header("SFX")]
    //private FMOD.Studio.EventInstance instance;
    //[FMODUnity.EventRef]
    [SerializeField] string footstepSounds;

    //Other Private Variables
    private Rigidbody2D rb2d;
    private Collider2D col;
    private PlayerHealth health;
    [HideInInspector] public Animator anim;
    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool flipPlayer;
    [HideInInspector] public bool playerDead;
    private Vector3 horizontalMovement = Vector3.zero;
    private bool flipRightInput;
    private bool flipLeftInput;
    private bool jumpInput;
    private bool playerCrouched;
    private bool playerSliding;
    private PlayerControls playerControls;
    [SerializeField] private LayerMask groundLayer;
    private float timeTillNextSlide;
    private bool currentlyFalling;
    private bool flipping;
    private bool wasGrounded;
    private float currentAFKTime;
    private float jumpTimer;
    private float coyoteCounter;
    private bool grounded;
    [HideInInspector] public bool externalForce;
    private float flipThreshholdTimer;
    private bool flipInsteadOfJump;
    private ParticleSystem.EmissionModule footEmission;
    [HideInInspector] public bool shootingEnabled = true;
    #endregion

    #region Monobehaviour Callbacks 

    private void Awake()
    {
        #region Components

        //Setting up Component references
        rb2d = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
        playerControls = new PlayerControls();

        #endregion
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Start()
    {
        #region InitializeValues
        //Initalizes private currentAfkTime to the AFKTimer set in inspector
        currentAFKTime = AFKTimer;

        //Allows us to access the emission variables for our dust particle effect.
        footEmission = dust.emission;

        #endregion

        #region PlayerInputEvents
        //----------------------------------PLAYER INPUT EVENTS---------------------------------------// 

        //Jump
        playerControls.Gameplay.Jump.performed += _ => jumpTimer = Time.time + jumpDelay;

        //Crouch
        playerControls.Gameplay.Crouch.performed += _ => Crouch();
        playerControls.Gameplay.Crouch.canceled += _ =>
        {
            playerCrouched = false;
            anim.SetBool("Crouch", false);
        };

        //SlideLeft
        playerControls.Gameplay.SlideLeft.performed += _ => StartCoroutine(SlideLeft());

        //SlideRight
        playerControls.Gameplay.SlideRight.performed += _ => StartCoroutine(SlideRight());

        //Left Flip
        playerControls.Gameplay.FlipLeft.started += _ => flipThreshholdTimer = 0;

        //Right Flip
        playerControls.Gameplay.FlipRight.started += _ => flipThreshholdTimer = 0;

        #endregion
    }
    private void Update()
    {
        Debug.Log(externalForce);
        //If the player is moving and stops super quickly and jumps, there is a brief period where 2 buttons are being pressed (space and either a or d).
        //This would trigger the flip animation even when you want to jump. To eliminate this, a flip threshhold is created to specify a certain amount
        //of time the flip input must be help to be registered as a flip and not a jump.
        DetermineIfPastFlipThreshhold();
        UpdateGroundedCheck(); //Checks and updates the grounded boolean
        GetWalkingInput(); //Gets the horizontal walking input only
        StopFrictionIfGrounded();  //Removes friction so the player doesn't get stuck on walls
        UpdateCoyoteCounter();  //Updates the coyoteCounter to allow hangtime when jumping off ledges.
        ResetInputs();  //Resets the jump, rightFlip, and leftFlip inputs to false.
        GetJumpOrFlipInput(); //Determines if the input pressed should be a jump, leftFlip, or rightFlip

        //Flip Player and leaves dust effect if mouse goes to the other side of the player. Determined in PlayerWeapon Script.
        if (flipPlayer)
        {
            ChangeDirection();
        }

        //Decrements the slide timer. When it hits 0, the player is allowed to slide again.
        UpdateTimeTillNextSlide();

        //Falling Animation
        WhenFallingPlayAnimation();

        //Landing Animations
        WhenLandingPlayAnimation();
    }

    private void FixedUpdate()
    {
        //Modifies the gravity for the physics to give a good snappy feel to the jump (normal going upwards, and high gravity going down)
        ModifyGravityPhysics();

        //Performs the basic horizontal movement (left and right only)
        PerformWalking();

        //Performs either a jump or a flip depending on which was inputted by the player.
        PerformJumpOrFlip();
    }

    public IEnumerator WaitToMove()
    {
        yield return new WaitForSeconds(1);
        shootingEnabled = true;
        externalForce = false;

    }

    #region Private Methods
    private void GetWalkingInput()
    {
        horizontalMovement.x = playerControls.Gameplay.MoveHorizontal.ReadValue<float>();
    }

    private void GetJumpOrFlipInput()
    {
        if (jumpTimer > Time.time && !externalForce)
        {
            if (horizontalMovement.x > 0 && coyoteCounter > 0 && flipInsteadOfJump)
                flipRightInput = true;
            else if (horizontalMovement.x < 0 && coyoteCounter > 0 && flipInsteadOfJump)
                flipLeftInput = true;
            else if (horizontalMovement.x == 0 && grounded)
                jumpInput = true;
        }
    }

    private void PerformWalking()
    {
        if (horizontalMovement.x != 0 && !externalForce)
        {
            if (grounded)
            {
                anim.SetFloat("Walking", Mathf.Abs(horizontalMovement.x));
            }
            if (playerCrouched)
                rb2d.velocity = new Vector2(horizontalMovement.x * maxSpeed / 2, rb2d.velocity.y);
            else
                rb2d.velocity = new Vector2(horizontalMovement.x * maxSpeed, rb2d.velocity.y);
        }
        else if (!externalForce)
        {
            anim.SetFloat("Walking", 0);
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
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

        //Show footstep effect only when moving and grounded
        if (horizontalMovement.x != 0 && grounded)
            footEmission.rateOverTime = dustEmissionRate;
        else
            footEmission.rateOverTime = 0f;
    }

    private void PerformJumpOrFlip()
    {
        if (flipRightInput)
            FlipRight();
        else if (flipLeftInput)
            FlipLeft();
        else if (jumpInput)
            Jump();
    }

    private void Jump()
    {
        if (!flipping)
        {
            anim.SetTrigger("Jump");
            rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
            rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpTimer = 0;
        }
    }
    private void Crouch()
    {
        if ((rb2d.velocity.x == 0 || MovingBackwards()) && !playerSliding && !externalForce)
        {
            playerCrouched = true;
            anim.SetBool("Crouch", true);
        }
    }

    private void FlipLeft()
    {
        if (!flipping)
        {
            shootingEnabled = false;
            flipping = true;
            if (PlayerIsFacingRight())
            {
                anim.SetTrigger("Backflip");
                rb2d.AddForce(new Vector2(-backflipForceX, backflipForceY));
            }
            else if (!PlayerIsFacingRight())
            {
                anim.SetTrigger("Somersault");
                rb2d.AddForce(new Vector2(-somersaultForceX, somersaultForceY));
            }
        }
    }
    private void FlipRight()
    {
        if (!flipping)
        {
            shootingEnabled = false;
            flipping = true;
            if (PlayerIsFacingRight())
            {
                anim.SetTrigger("Somersault");
                rb2d.AddForce(new Vector2(somersaultForceX, somersaultForceY));
            }
            else if (!PlayerIsFacingRight())
            {
                anim.SetTrigger("Backflip");
                rb2d.AddForce(new Vector2(backflipForceX, backflipForceY));
            }
        }
    }
    private IEnumerator SlideRight()
    {
        if (grounded && !externalForce && timeTillNextSlide <= 0 && !playerSliding && !playerCrouched)
        {
            if (PlayerIsFacingRight())
            {
                rb2d.velocity = new Vector2(0, rb2d.velocity.y);
                externalForce = true;
                playerSliding = true;
                anim.SetBool("Slide", true);
                rb2d.velocity = new Vector2(slideForce, rb2d.velocity.y);
                yield return new WaitForSeconds(slideDuration);
                rb2d.velocity = Vector2.zero;
                externalForce = false;
                playerSliding = false;
                anim.SetBool("Slide", false);
                timeTillNextSlide = timeBtwSlides;
            }
        }
    }
    private IEnumerator SlideLeft()
    {
        if (grounded && !externalForce && timeTillNextSlide <= 0 && !playerSliding && !playerCrouched)
        {
            if (!PlayerIsFacingRight())
            {
                rb2d.velocity = new Vector2(0, rb2d.velocity.y);
                externalForce = true;
                playerSliding = true;
                anim.SetBool("Slide", true);
                rb2d.velocity = new Vector2(-slideForce, rb2d.velocity.y);
                yield return new WaitForSeconds(slideDuration);
                rb2d.velocity = Vector2.zero;
                timeTillNextSlide = timeBtwSlides;
                externalForce = false;
                playerSliding = false;
                anim.SetBool("Slide", false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //THIS IS ONLY EXECUTED WHEN THE PLAYER IS HIT BY A BLADE
        if (collision.gameObject.layer == 15)
        {
            if (!collision.gameObject.GetComponent<Blade>().isStationary)
            {
                health.ModifyHealth(-1);
                shootingEnabled = false;
                externalForce = true;
                rb2d.velocity = new Vector2(-knockbackStrength, -10);
                if (facingRight)
                    anim.SetTrigger("Fall Back");
                else
                    anim.SetTrigger("Fall Forward");
                Destroy(collision.gameObject);
                StartCoroutine(WaitAndStand());
            }
        }
    }

    IEnumerator WaitAndStand()
    {
        bool looping = true;
        while (looping)
        {
            if (grounded)
            {
                yield return new WaitForSeconds(1);
                shootingEnabled = true;
                if (facingRight)
                    anim.SetTrigger("Stand Up");
                else
                    anim.SetTrigger("StandUp_Forward");
                looping = false;
                externalForce = false;
            }
            else
                yield return new WaitForSeconds(0.01f);
        }
    }

    private void DetermineIfPastFlipThreshhold()
    {
        if (flipThreshholdTimer > .04f)
            flipInsteadOfJump = true;
        else
            flipInsteadOfJump = false;
        flipThreshholdTimer += Time.deltaTime;
    }

    private void WhenFallingPlayAnimation()
    {
        if (!currentlyFalling)
        {
            if (PlayerFalling())
                anim.SetTrigger("Falling");
        }
    }

    private void WhenLandingPlayAnimation()
    {
        if (PlayerLanding())
        {
            impactEffect.gameObject.SetActive(true);
            impactEffect.Stop();
            impactEffect.transform.position = dust.transform.position;
            impactEffect.Play();
            anim.SetTrigger("Landing");
        }
    }
    private void UpdateCoyoteCounter()
    {
        if (grounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;
    }
    private void StopFrictionIfGrounded()
    {
        if (grounded)
            rb2d.sharedMaterial = frictionMaterial;
        else
            rb2d.sharedMaterial = noFrictionMaterial;
    }
    private void UpdateGroundedCheck()
    {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, extraHeight, groundLayer);
        Color rayColor;
        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
            rayColor = Color.red;

        Debug.DrawRay(col.bounds.center + new Vector3(col.bounds.extents.x, 0), Vector2.down * (col.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(col.bounds.center - new Vector3(col.bounds.extents.x, 0), Vector2.down * (col.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(col.bounds.center - new Vector3(0, col.bounds.extents.x, col.bounds.extents.y + extraHeight), Vector2.right * (col.bounds.extents.x), rayColor);

        //Manage hang time 
        if (raycastHit.collider != null) { //If grounded (raycast hit something)
            if (rb2d.velocity.y <= 0) //Set flipping to false once you land. the == 0 accounts for flipping into a wall where your y velocity never becomes negative and landing is never triggered.
                flipping = false;
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
            wasGrounded = true;
        }

        grounded = (raycastHit.collider != null);
    }
    private void ModifyGravityPhysics()
    {
        if (grounded && rb2d.velocity.y == 0)
        {
            rb2d.gravityScale = 0;
        }
        else
        {
            rb2d.gravityScale = gravity;
            if (rb2d.velocity.y < 0) //falling
            {
                rb2d.gravityScale = gravity * fallMultiplier;
            }
            else if (rb2d.velocity.y > 0 && Keyboard.current.spaceKey.isPressed && !flipping) //jump is held down
            {
                rb2d.gravityScale = gravity * (fallMultiplier / 5.5f);
            }
        }
    }

    private bool PlayerIsFacingRight()
    {
        bool facingRight = true;
        Vector3 mouseInput = Camera.main.ScreenToWorldPoint(playerControls.Gameplay.Aim.ReadValue<Vector2>());
        if (mouseInput.x < transform.position.x)
            facingRight = false;
        else if (mouseInput.y > transform.position.x)
            facingRight = true;
        return facingRight;
    }
    private void ChangeDirection()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;

        firePoint.Rotate(0f, 180f, 0f);
    }

    private bool MovingBackwards()
    {
        if (facingRight && rb2d.velocity.x < 0)
            return true;
        else if (!facingRight && rb2d.velocity.x > 0)
            return true;
        else
            return false;
    }

    private void UpdateTimeTillNextSlide()
    {
        if (timeTillNextSlide > 0)
        {
            timeTillNextSlide -= Time.deltaTime;
        }
    }

    private bool PlayerFalling()
    {
        //-2.8 chosen instead of 0 to make sure player is actually falling. Before sometimes it would glitch and trigger falling even when player wasn't actually falling.
        if (rb2d.velocity.y < -2.8 && !grounded && !flipping && !externalForce)
        {
            currentlyFalling = true;
            return true;
        }
        else
        {
            currentlyFalling = false;
            return false;
        }
    }

    private bool PlayerLanding()
    {
        if (wasGrounded && grounded && rb2d.velocity.y == 0 && !externalForce) //came from the air, now grounded
        {
            shootingEnabled = true;
            currentlyFalling = false;
            wasGrounded = false;
            flipping = false;
            return true;
        }
        else
            return false;
    }

    private void ResetInputs()
    {
        jumpInput = false;
        flipRightInput = false;
        flipLeftInput = false;
    }
    #endregion

    #endregion
}