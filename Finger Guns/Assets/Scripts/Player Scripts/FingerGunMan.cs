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

    //Private Variables
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
    private bool crouchInput;
    private bool leftSlideInput;
    private bool rightSlideInput;
    private bool playerCrouched;
    private bool playerSliding;
    private PlayerControls playerControls;
    [SerializeField] private LayerMask groundLayer;
    private float timeTillNextSlide;
    private Coroutine co;
    private bool currentlyFalling;
    private bool slowerMovementInAir;
    private bool flipping;
    private bool inBackflip;
    private bool inSomersault;
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
        playerControls.Gameplay.Jump.performed += _ =>
        {
            jumpTimer = Time.time + jumpDelay;
            slowerMovementInAir = true;
        };

        //Crouch
        playerControls.Gameplay.Crouch.started += _ => crouchInput = true;
        playerControls.Gameplay.Crouch.canceled += _ =>
        {
            crouchInput = false;
            playerCrouched = false;
            anim.SetBool("Crouch", false);
        };

        //SlideLeft
        playerControls.Gameplay.SlideLeft.performed += _ => leftSlideInput = true;
        playerControls.Gameplay.SlideLeft.canceled += _ =>
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            if (co != null)
                StopCoroutine(co);
            timeTillNextSlide = timeBtwSlides;
            leftSlideInput = false;
            playerSliding = false;
            anim.SetBool("Slide", false);
        };

        //SlideRight
        playerControls.Gameplay.SlideRight.performed += _ => rightSlideInput = true;
        playerControls.Gameplay.SlideRight.canceled += _ =>
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            if (co != null)
                StopCoroutine(co);
            timeTillNextSlide = timeBtwSlides;
            rightSlideInput = false;
            playerSliding = false;
            anim.SetBool("Slide", false);
        };

        //Left Flip
        playerControls.Gameplay.FlipLeft.started += _ => flipThreshholdTimer = 0;

        //Right Flip
        playerControls.Gameplay.FlipRight.started += _ => flipThreshholdTimer = 0;

        #endregion
    }
    private void Update()
    {
        //Checks to see if flipping animation is done. If it is, booleans are updated
        CheckIfEndOfFlipAnim();

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
        PerformCrouch();  //Checks if the player is crouched, if they are the crouch animation is executed

        //Flip Player and leaves dust effect if mouse goes to the other side of the player. Determined in PlayerWeapon Script.
        if (flipPlayer)
        {
            ChangeDirection(); ;
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

        //Checks if the player is sliding, if they are ethe slide animation is executed.
        PerformSlide();
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
            if (grounded && !playerCrouched && !flipping) //When you're on the ground
            {
                //Debug.Log("Going normal.");
                anim.SetFloat("Walking", Mathf.Abs(horizontalMovement.x));
                rb2d.velocity = new Vector2(horizontalMovement.x * maxSpeed, rb2d.velocity.y); //Go normal speed when on the ground
            }
            else if ((inSomersault && rb2d.velocity.x > 0 && facingRight) || (inSomersault && rb2d.velocity.x < 0 && !facingRight)) //When you Somersault and are facing the same direction (not moving backwards after you somersault)
            {
                //Debug.Log("Going faster.");
                rb2d.velocity = new Vector2(horizontalMovement.x * maxSpeed * 1.5f, rb2d.velocity.y); //Go slightly faster when you somersault
            }
            else if ((playerCrouched && !playerSliding) || inBackflip || slowerMovementInAir) //If you are either crouched, in a backflip, or jumping (jumping turns on slowerMovementInAir)
            {
                //Debug.Log("Going slower.");
                rb2d.velocity = new Vector2(horizontalMovement.x * maxSpeed / 2, rb2d.velocity.y);  //Go half as fast when either of these are performed
            }
            else
            {
                //Debug.Log("Going normal.");
                rb2d.velocity = new Vector2(horizontalMovement.x * maxSpeed, rb2d.velocity.y); //If all these are false, go normal speed
            }
        }
        else if (!externalForce) //When no input, set walking speed back to 0
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

    private void PerformCrouch()
    {
        if (crouchInput && !playerCrouched && grounded)
            Crouch();
    }

    private void PerformSlide()
    {
        if (leftSlideInput && !playerSliding && !flipping)
            co = StartCoroutine(SlideLeft());
        else if (rightSlideInput && !playerSliding && !flipping)
            co = StartCoroutine(SlideRight());
    }

    private void Jump()
    {
        if (!flipping && !playerSliding)
        {
            anim.SetBool("Crouch", false);
            playerCrouched = false;
            anim.SetTrigger("Jump");
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpTimer = 0;
        }
    }
    private void Crouch()
    {
        if ((rb2d.velocity.x == 0 || MovingBackwards()) && !playerSliding && !externalForce && grounded)
        {
            playerCrouched = true;
            anim.SetBool("Crouch", true);
        }
    }

    private void FlipLeft()
    {
        if (!flipping && !playerSliding)
        {
            rb2d.velocity = Vector2.zero;
            anim.SetBool("Crouch", false);
            playerCrouched = false;
            shootingEnabled = false;
            flipping = true;
            if (PlayerIsFacingRight())
            {
                anim.SetTrigger("Backflip");
                rb2d.AddForce(new Vector2(-backflipForceX, backflipForceY), ForceMode2D.Impulse);
                inBackflip = true;
            }
            else if (!PlayerIsFacingRight())
            {
                anim.SetTrigger("Somersault");
                rb2d.AddForce(new Vector2(-somersaultForceX, somersaultForceY), ForceMode2D.Impulse);
                inSomersault = true;
            }
        }
    }
    private void FlipRight()
    {
        if (!flipping && !playerSliding)
        {
            rb2d.velocity = Vector2.zero;
            anim.SetBool("Crouch", false);
            playerCrouched = false;
            shootingEnabled = false;
            flipping = true;
            if (PlayerIsFacingRight())
            {
                anim.SetTrigger("Somersault");
                rb2d.AddForce(new Vector2(somersaultForceX, somersaultForceY), ForceMode2D.Impulse);
                inSomersault = true;
            }
            else if (!PlayerIsFacingRight())
            {
                anim.SetTrigger("Backflip");
                rb2d.AddForce(new Vector2(backflipForceX, backflipForceY), ForceMode2D.Impulse);
                inBackflip = true;
            }
        }
    }
    private void CheckIfEndOfFlipAnim()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("FingerGunMan_Rig|Somersault") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - jumpDelay)
        {
            inSomersault = false;
            flipping = false;
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("FingerGunMan_Rig|Backflip") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - jumpDelay)
        {
            inBackflip = false;
            flipping = false;
        }
    }
    private IEnumerator SlideRight()
    {
        if (grounded && !externalForce && timeTillNextSlide <= 0 && !playerCrouched)
        {
            if (PlayerIsFacingRight())
            {
                playerSliding = true;
                anim.SetBool("Slide", true);
                rb2d.velocity = new Vector2(slideForce, rb2d.velocity.y);
                yield return new WaitForSeconds(slideDuration);
                rightSlideInput = false;
                playerSliding = false;
                anim.SetBool("Slide", false);
                timeTillNextSlide = timeBtwSlides;
            }
        }
    }
    private IEnumerator SlideLeft()
    {
        if (grounded && !externalForce && timeTillNextSlide <= 0 && !playerCrouched)
        {
            if (!PlayerIsFacingRight())
            {
                playerSliding = true;
                anim.SetBool("Slide", true);
                rb2d.velocity = new Vector2(-slideForce, rb2d.velocity.y);
                yield return new WaitForSeconds(slideDuration);
                leftSlideInput = false;
                playerSliding = false;
                anim.SetBool("Slide", false);
                timeTillNextSlide = timeBtwSlides;
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
                anim.SetBool("Crouch", false);
                anim.SetBool("Slide", false);
                playerCrouched = false;
                playerSliding = false;
                flipping = false;
                inBackflip = false;
                inSomersault = false;
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
        yield return new WaitForSeconds(0.7f);
        while (looping)
        {
            if (grounded)
            {
                shootingEnabled = true;
                if (facingRight)
                    anim.SetTrigger("Stand Up");
                else
                    anim.SetTrigger("StandUp_Forward");
                looping = false;
                yield return new WaitForSeconds(.2f);
                externalForce = false;
            }
            else
                yield return new WaitForSeconds(0.01f);
        }
    }

    private void DetermineIfPastFlipThreshhold()
    {
        if (flipThreshholdTimer > .05f)
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
        if (grounded && !flipping)
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
        if (raycastHit.collider != null)
        { //If grounded (raycast hit something)
            //if (rb2d.velocity.y <= 0) //Set flipping to false once you land. the == 0 accounts for flipping into a wall where your y velocity never becomes negative and landing is never triggered.
                //flipping = false; //need to change when flipping is set to false
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
        if (rb2d.velocity.y < 0 && !grounded && !flipping && !externalForce)
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
        if (wasGrounded && grounded && rb2d.velocity.y == 0 && !externalForce && !flipping) //came from the air, now grounded
        {
            shootingEnabled = true;
            currentlyFalling = false;
            wasGrounded = false;
            slowerMovementInAir = false;
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