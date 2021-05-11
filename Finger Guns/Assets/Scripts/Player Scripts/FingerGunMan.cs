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
    [Header("Testing Variables")]
    [SerializeField] bool allowFlipDodging;
    [Space()]
    [Header("BodyParts")]
    [SerializeField] Transform head;
    [SerializeField] Transform feet;
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
    [SerializeField] float maxFallSpeed = 15f;
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
    [Header("GroundLayer")]
    [SerializeField] LayerMask groundLayer;
    [Space()]
    [Header("SFX")]
    //private FMOD.Studio.EventInstance instance;
    //[FMODUnity.EventRef]
    [SerializeField] string footstepSounds;

    //Components and References
    private Rigidbody2D rb2d;
    private Collider2D col;
    private PlayerHealth health;
    private Wind wind;

    //Private Variables
    private Vector3 horizontalMovement = Vector3.zero;
    private bool flipRightInput;
    private bool flipLeftInput;
    private bool jumpInput;
    private bool crouchInput;
    private Coroutine co;
    private bool leftSlideInput;
    private bool rightSlideInput;
    private bool playerCrouched;
    private bool playerSliding;
    private PlayerControls playerControls;
    private float timeTillNextSlide;
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
    private float flipThreshholdTimer;
    private bool flipInsteadOfJump;
    private ParticleSystem.EmissionModule footEmission;

    #endregion

    #region Monobehaviour Callbacks 

    private void Awake()
    {
        #region Components

        //Setting up Component references
        rb2d = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        Anim = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
        wind = FindObjectOfType<Wind>();
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

        defaultMaxSpeed = maxSpeed; //assigns the defaultMaxSpeed to maxSpeed. Default max speed needed to revert to original maxSpeed once maxSpeed is altered

        #endregion

        #region PlayerInputEvents
        //----------------------------------PLAYER INPUT EVENTS---------------------------------------// 

        //Jump
        playerControls.Gameplay.Jump.started += _ =>
        {
            jumpTimer = Time.time + jumpDelay; //Starts the timer at whatever the current time is + what you set as the jump delay
            slowerMovementInAir = true;  //Since jumps should cause slower movement in the air, a boolean is used to indicate this
        };

        //Crouch
        playerControls.Gameplay.Crouch.started += _ => crouchInput = true;
        playerControls.Gameplay.Crouch.canceled += _ =>
        {
            crouchInput = false;
            rightSlideInput = false;
            leftSlideInput = false;
            playerCrouched = false;
            Anim.SetBool("Crouch", false);
        };

        //SlideLeft
        playerControls.Gameplay.SlideLeft.started += _ => leftSlideInput = true;

        //SlideRight
        playerControls.Gameplay.SlideRight.started += _ => rightSlideInput = true;

        //Left Flip
        playerControls.Gameplay.FlipLeft.started += _ => flipThreshholdTimer = 0;

        //Right Flip
        playerControls.Gameplay.FlipRight.started += _ => flipThreshholdTimer = 0;

        #endregion
    }
    private void Update()
    {
        UpsideDownCheck(); //checks if the player is upside down. Important so player isn't seen as grounded when he's upside down and touching the floor

        CheckIfEndOfFlipAnim(); //Checks to see if flipping Animation is done. If it is, booleans are updated

        //If the player is moving and stops super quickly and jumps, there is a brief period where 2 buttons are being pressed (space and either a or d).
        //This would trigger the flip Animation even when you want to jump. To eliminate this, a flip threshhold is created to specify a certain amount
        //of time the flip input must be help to be registered as a flip and not a jump.
        DetermineIfPastFlipThreshhold();

        UpdateGroundedCheck(); //Checks and updates the grounded boolean. Also updates the coyote counter variable to allow coyote jumping

        GetWalkingInput(); //Gets the horizontal walking input only

        StopFrictionIfNotGrounded();  //Removes friction so the player doesn't get stuck on walls

        ResetInputs();  //Resets the jump, rightFlip, and leftFlip inputs to false.

        GetJumpOrFlipInput(); //Determines if the input pressed should be a jump, leftFlip, or rightFlip

        PerformCrouch();  //Checks if the player is crouched, if they are the crouch Animation is executed

        //Flip Player and leaves dust effect if mouse goes to the other side of the player. Determined in PlayerWeapon Script.
        if (FlipPlayer)
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

        //Checks if the player is sliding, if they are ethe slide Animation is executed.
        PerformSlide();

        //If you are doing a somersault and not moving, then you are caught on something and velocity should be set to 0
        if (horizontalMovement.x != 0 && inSomersault && Mathf.Abs(rb2d.velocity.x) < 10 && !playerSliding)
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);

        }
    }

    //Coroutine started when the player is struck by lightning. They can't shoot or move for 1 second.
    public IEnumerator WaitToMove()
    {
        yield return new WaitForSeconds(1);
        ShootingEnabled = true;
        ExternalForce = false;
    }

    #region Private Methods
    private void GetWalkingInput()
    {
        horizontalMovement.x = playerControls.Gameplay.MoveHorizontal.ReadValue<float>();
    }

    private void GetJumpOrFlipInput()
    {
        if (jumpTimer > Time.time && !ExternalForce)
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
        maxSpeed = defaultMaxSpeed; //Sets maxSpeed back to default so sum doesn't accumulate every frame if the wind is active.

        if (horizontalMovement.x != 0 && !ExternalForce && !playerSliding && health.GetHealth() > 0)  //If external force is enabled or if (enabled by any obstacle such as a blade or lightning), or the player is dead, the player is unable to move
        {
            if (wind != null && wind.WindActive)
            {
                if ((horizontalMovement.x > 0 && wind.currentWindForce < 0) || (horizontalMovement.x < 0 && wind.currentWindForce > 0)) //If wind is opposing your movement, you go slower
                    maxSpeed -= Mathf.Abs(wind.currentWindForce);
                else if ((horizontalMovement.x > 0 && wind.currentWindForce > 0) || (horizontalMovement.x < 0 && wind.currentWindForce < 0)) //If wind in the same direction as your movement, you go faster.
                    maxSpeed += Mathf.Abs(wind.currentWindForce);
            }

            if (grounded && !playerCrouched && !flipping)
            {
                Anim.SetFloat("Walking", Mathf.Abs(horizontalMovement.x));
                rb2d.velocity = new Vector2(horizontalMovement.x * maxSpeed, rb2d.velocity.y); //Go normal speed when on the ground
            }
            else if ((inSomersault && rb2d.velocity.x > 0 && FacingRight) || (inSomersault && rb2d.velocity.x < 0 && !FacingRight)) //When you Somersault and are facing the same direction (not moving backwards after you somersault)
            {
                rb2d.velocity = new Vector2(horizontalMovement.x * maxSpeed * 1.5f, rb2d.velocity.y); //Go slightly faster when you somersault
            }
            else if (playerCrouched || inBackflip || slowerMovementInAir) //If you are either crouched, in a backflip, or jumping (jumping turns on slowerMovementInAir)
            {
                rb2d.velocity = new Vector2(horizontalMovement.x * maxSpeed / 2, rb2d.velocity.y);  //Go half as fast when either of these are performed
            }
            else
            {
                rb2d.velocity = new Vector2(horizontalMovement.x * maxSpeed, rb2d.velocity.y); //If all these are false, go normal speed
            }
        }
        else if (!ExternalForce && !playerSliding) //When no input, set walking speed back to 0
        {
            if (wind != null)
            {
                Anim.SetFloat("Walking", 0 + wind.currentWindForce);
                rb2d.velocity = new Vector2(0 + wind.currentWindForce, rb2d.velocity.y);
            }
            else
            {
                Anim.SetFloat("Walking", 0);
                rb2d.velocity = new Vector2(0, rb2d.velocity.y);
            }
            if (currentAFKTime <= 0)
            {
                Anim.SetTrigger("AFK");
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
        if (!flipping && !playerSliding) //!flipping is in the condition so this method isn't continuously called. Only want to call it the first time, when the player isn't flipping and not sliding
        {
            if (flipRightInput)
            {
                if (allowFlipDodging)
                {
                    Physics2D.IgnoreLayerCollision(10, 13, true);  //Ignores enemy bullets during right flips if the allowFlipDodging boolean is turned on in the inspector
                }
                FlipRight(); //Performs the actual right flip Animation
            }
            else if (flipLeftInput)
            {
                if (allowFlipDodging)
                {
                    Physics2D.IgnoreLayerCollision(10, 13, true); //Ignores enemy bullets during left flips if the allowFlipDodging boolean is turned on in the inspector
                }
                FlipLeft();  //Performs the actual left flip Animation
            }
            else if (jumpInput)
                Jump();  //Performs the actual jump Animation
        }
    }

    private void PerformCrouch()
    {
        //Want to be able to crouch only if all these variables are true
        if (crouchInput && !playerCrouched && grounded && !ExternalForce && (rb2d.velocity.x == 0 || MovingBackwards()))
            Crouch();
    }

    private void PerformSlide()
    {
        //If the player is sliding but runs into a wall and stops. Want to cancel the slide Animation. < 5 is used instead of 0 because sometimes the velocity isn't exactly 0
        if ((playerSliding && FacingRight && rb2d.velocity.x < 5) || (playerSliding && !FacingRight && rb2d.velocity.x > -5))
        {
            leftSlideInput = false;
            rightSlideInput = false;
            playerSliding = false;
            Anim.SetBool("Slide", false);
        }
        else if (playerSliding && Keyboard.current.wKey.isPressed) //Cancels the slide Animation
        {
            Anim.SetBool("Slide", false);
            playerSliding = false;
            leftSlideInput = false;
            rightSlideInput = false;
            timeTillNextSlide = timeBtwSlides;
        }
        else if (!playerSliding && !playerCrouched && !ExternalForce && grounded && timeTillNextSlide <= 0)
        {
            if (rightSlideInput)
                co = StartCoroutine(SlideRight());
            else if (leftSlideInput)
                co = StartCoroutine(SlideLeft());
        }
    }

    private void Jump()
    {

        Anim.SetBool("Crouch", false);
        playerCrouched = false;
        Anim.SetTrigger("Jump");
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        jumpTimer = 0;
    }
    private void Crouch()
    {
        flipping = false;
        inSomersault = false;
        inBackflip = false;
        playerCrouched = true;
        Anim.SetBool("Crouch", true);
    }

    private void FlipLeft()
    {
        Anim.SetBool("Crouch", false);
        playerCrouched = false;
        ShootingEnabled = false;
        flipping = true;
        if (PlayerIsFacingRight())
        {
            Anim.SetTrigger("Backflip");
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(-backflipForceX, backflipForceY), ForceMode2D.Impulse);
            inBackflip = true;
        }
        else if (!PlayerIsFacingRight())
        {
            Anim.SetTrigger("Somersault");
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(-somersaultForceX, somersaultForceY), ForceMode2D.Impulse);
            inSomersault = true;
        }
    }
    private void FlipRight()
    {
        Anim.SetBool("Crouch", false);
        playerCrouched = false;
        ShootingEnabled = false;
        flipping = true;
        if (PlayerIsFacingRight())
        {
            Anim.SetTrigger("Somersault");
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(somersaultForceX, somersaultForceY), ForceMode2D.Impulse);
            inSomersault = true;
        }
        else if (!PlayerIsFacingRight())
        {
            Anim.SetTrigger("Backflip");
            rb2d.velocity = Vector2.zero;
            rb2d.AddForce(new Vector2(backflipForceX, backflipForceY), ForceMode2D.Impulse);
            inBackflip = true;
        }
    }
    private void CheckIfEndOfFlipAnim()
    {
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName("FingerGunMan_Rig|Somersault") && Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - jumpDelay)
        {
            Physics2D.IgnoreLayerCollision(10, 13, false); //If at end of somersault Animation, allow the player to be hit by enemy projectiles again.
            inSomersault = false;
            flipping = false;
        }
        else if (Anim.GetCurrentAnimatorStateInfo(0).IsName("FingerGunMan_Rig|Backflip") && Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f - jumpDelay)
        {
            Physics2D.IgnoreLayerCollision(10, 13, false); //If at end of backflip Animation, allow the player to be hit by enemy projectiles again.
            inBackflip = false;
            flipping = false;
        }
    }
    private IEnumerator SlideRight()
    {
        if (PlayerIsFacingRight())
        {
            flipping = false;
            inSomersault = false;
            inBackflip = false;
            playerSliding = true;
            Anim.SetBool("Slide", true);
            rb2d.velocity = new Vector2(slideForce, rb2d.velocity.y);
            yield return new WaitForSeconds(slideDuration);
            rightSlideInput = false;
            playerSliding = false;
            Anim.SetBool("Slide", false);
            timeTillNextSlide = timeBtwSlides; //Sets the timer to the timebtwSlide time declared in the inspector. This will count down and once it hits 0, the player can slide again.
        }
    }
    private IEnumerator SlideLeft()
    {
        if (!PlayerIsFacingRight())
        {
            flipping = false;
            inSomersault = false;
            inBackflip = false;
            playerSliding = true;
            Anim.SetBool("Slide", true);
            rb2d.velocity = new Vector2(-slideForce, rb2d.velocity.y);
            yield return new WaitForSeconds(slideDuration);
            leftSlideInput = false;
            playerSliding = false;
            Anim.SetBool("Slide", false);
            timeTillNextSlide = timeBtwSlides; //Sets the timer to the timebtwSlide time declared in the inspector. This will count down and once it hits 0, the player can slide again.
        }
    }
    private void UpsideDownCheck()
    {
        if ((Mathf.Round((head.position.y - feet.position.y) * 100f) / 100f) < 1.75f && !ExternalForce) //if the y position between the head and feet is < 1.75 units, player is declared as upside down
            PlayerUpsideDown = true;
        else
            PlayerUpsideDown = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //THIS IS ONLY EXECUTED WHEN THE PLAYER IS HIT BY A BLADE
        if (collision.gameObject.layer == 15)
        {
            if (!collision.gameObject.GetComponent<Blade>().IsStationary)
            {
                health.ModifyHealth(-1);
                Anim.SetBool("Crouch", false);
                Anim.SetBool("Slide", false);
                playerCrouched = false;
                playerSliding = false;
                flipping = false;
                inBackflip = false;
                inSomersault = false;
                ShootingEnabled = false;
                ExternalForce = true; //External force means the player cannot move
                rb2d.velocity = new Vector2(-knockbackStrength, -10);
                if (FacingRight)
                    Anim.SetTrigger("Fall Back");
                else
                    Anim.SetTrigger("Fall Forward");
                Destroy(collision.gameObject); //Destroy blade after player is hit
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
                ShootingEnabled = true;
                if (FacingRight)
                    Anim.SetTrigger("Stand Up");
                else
                    Anim.SetTrigger("StandUp_Forward");
                looping = false;
                yield return new WaitForSeconds(.2f);
                ExternalForce = false;
            }
            else
                yield return new WaitForSeconds(0.01f);
        }
    }

    private void DetermineIfPastFlipThreshhold()
    {
        if (flipThreshholdTimer > .05f) //If either a or d is held down for more than .05 seconds, then it is registred as a flip and not a jump. Needed so when player abruptly stops, it isn't interpreted as a flip
            flipInsteadOfJump = true;
        else
            flipInsteadOfJump = false;
        flipThreshholdTimer += Time.deltaTime;
    }

    private void WhenFallingPlayAnimation()
    {
        if (!currentlyFalling) //currently falling is used just so the falling Animation isn't called multiple times
        {
            if (PlayerFalling())
                Anim.SetTrigger("Falling");
        }
    }

    //Plays the impact particle effect when the player lands
    private void WhenLandingPlayAnimation()
    {
        if (PlayerLanding())
        {
            impactEffect.gameObject.SetActive(true);
            impactEffect.Stop();
            impactEffect.transform.position = dust.transform.position;
            impactEffect.Play();
            Anim.SetTrigger("Landing");
        }
    }

    private void StopFrictionIfNotGrounded()
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

        //If grounded (raycast hit something) and not upside down
        if (raycastHit.collider != null && !PlayerUpsideDown)
        {
            coyoteCounter = coyoteTime; //When on the ground the coyoteCounter is set back to the coyoteTime declared in the inspector.
            rayColor = Color.green;
        }
        else
        {
            coyoteCounter -= Time.deltaTime; //If not, the coyoteCounter is decremented. You can do a flip, even when not grounded, until the counter hits 0. 
            wasGrounded = true;
            rayColor = Color.red;
        }

        Debug.DrawRay(col.bounds.center + new Vector3(col.bounds.extents.x, 0), Vector2.down * (col.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(col.bounds.center - new Vector3(col.bounds.extents.x, 0), Vector2.down * (col.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(col.bounds.center - new Vector3(0, col.bounds.extents.x, col.bounds.extents.y + extraHeight), Vector2.right * (col.bounds.extents.x), rayColor);

        grounded = (raycastHit.collider != null && !PlayerUpsideDown);
    }
    private void ModifyGravityPhysics()
    {
        //Player can't fall above the max fall speed
        if (rb2d.velocity.y < 0 && rb2d.velocity.magnitude > maxFallSpeed)
            rb2d.velocity = Vector2.ClampMagnitude(rb2d.velocity, maxFallSpeed);

        if (grounded && rb2d.velocity.y == 0) //gravity scale set to 0 when on the ground and not moving
        {
            rb2d.gravityScale = 0;
        }
        else
        {
            rb2d.gravityScale = gravity;  //gravity scale set to normal gravity otherwise (like when you tap spacebar)
            if (rb2d.velocity.y < 0) //falling
            {
                rb2d.gravityScale = gravity * fallMultiplier;  //Makes you fall faster when you fall. Gives a better game feel.
            }
            else if (rb2d.velocity.y > 0 && Keyboard.current.spaceKey.isPressed && !flipping) //jump is held down
            {
                rb2d.gravityScale = gravity * (fallMultiplier / 5.5f);  //Gravity is decreased when you hold down spacebar, making you go higher.
            }
        }
    }

    private bool PlayerIsFacingRight()
    {
        bool FacingRight = true;
        Vector3 mouseInput = Camera.main.ScreenToWorldPoint(playerControls.Gameplay.Aim.ReadValue<Vector2>());
        if (mouseInput.x < transform.position.x)
            FacingRight = false;
        else if (mouseInput.y > transform.position.x)
            FacingRight = true;
        return FacingRight;
    }
    private void ChangeDirection()
    {
        if (co != null)
            StopCoroutine(co);

        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        FacingRight = !FacingRight;

        firePoint.Rotate(0f, 180f, 0f);
    }

    private bool MovingBackwards()
    {
        if (FacingRight && rb2d.velocity.x < 0)
            return true;
        else if (!FacingRight && rb2d.velocity.x > 0)
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
        if (rb2d.velocity.y < 0 && !grounded && !flipping && !ExternalForce)
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
        //if (wasGrounded && grounded && !ExternalForce)
        if (wasGrounded && grounded && rb2d.velocity.y == 0 && !ExternalForce && !flipping) //came from the air, now grounded
        {
            ShootingEnabled = true;
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

    #region Properties
    //Properties
    public bool ExternalForce { get; set; }
    public bool ShootingEnabled { get; set; } = true;
    public bool FacingRight { get; set; } = true;
    public bool FlipPlayer { get; set; }
    public bool PlayerDead { get; set; }
    public Animator Anim { get; set; }
    public bool PlayerUpsideDown { get; set; }
    public float playerXMovement { get { return horizontalMovement.x; } }
    public float defaultMaxSpeed { get; set; }

    #endregion

    #endregion
}