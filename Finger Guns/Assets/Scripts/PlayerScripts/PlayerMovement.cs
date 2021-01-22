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
    [Space()]

    [Header("Movement")]
    public float doubleTapWindow = 0.5f;
    public float movementSpeed = 100f;
    public float dashSpeed = 100f;
    public float jumpForce = 5f;
    public float somersaultForceX = 3f;
    public float somersaultForceY = 3f;
    public float backflipForceX = 3f;
    public float backflipForceY = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public bool facingRight = true;
    [Space()]
    public float AFKTimer = 10f;

    [Header("Weapon")]
    public Transform firePoint;    

    //Other Private Variables        
    private bool grounded;
    private bool wasGrounded;
    private bool falling;
    private int buttonCount = 0;
    private float horizontalInput;
    private bool jumpInput;
    private bool dashInput;    
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
            jumpInput = Input.GetButtonDown("Jump");
            //SomerSault
            somersaultInput = Input.GetKeyDown(KeyCode.S) && rb2d.velocity.x > 0;
            //Dash
            dashInput = GetDashInput();
            //Backflip
            backflipInput = Input.GetKeyDown(KeyCode.S) && rb2d.velocity.x < 0;

            //Stop falling animation if doing somersault or backflip
            if (somersaultInput || backflipInput)
                ignoreFalling = true;
        }
    }

    private bool GetDashInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {

            if (doubleTapWindow > 0 && buttonCount == 1/*Number of Taps you want Minus One*/)
            {
                //Has double tapped
                ignoreFalling = true;
                return true;
            }
            else
            {
                doubleTapWindow = 0.5f;
                buttonCount += 1;
            }
        }

        if (doubleTapWindow > 0)
        {

            doubleTapWindow -= 1 * Time.deltaTime;

        }
        else
        {
            buttonCount = 0;
        }
        return false;
    }

    private void PerformMovement()
    {
        //Ground Check
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, groundLayer);
        //Falling Check
        falling = ((rb2d.velocity.y < 0) && (!ignoreFalling) && (!grounded)) ? true : false;

        //Movement
        rb2d.velocity = new Vector2(horizontalInput * movementSpeed, rb2d.velocity.y); //Might have to put Time.deltaTime
        //Flip Player
        if(flipPlayer)
        {
            Flip();
        }

        //Dash
        if (dashInput && grounded)
        {
            transform.position += new Vector3(dashSpeed * Time.deltaTime, 0.1f, 0.0f);
        }

        //Jump
        if(jumpInput && grounded)
        {
            rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
        //SomerSault
        else if (somersaultInput && grounded)
        {
            rb2d.AddForce(new Vector2(somersaultForceX, somersaultForceY), ForceMode2D.Impulse);
        }
        else if (backflipInput && grounded)
        {
            rb2d.AddForce(new Vector2(-backflipForceX, backflipForceY), ForceMode2D.Impulse);
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

        //Jump, Fall, & Land
        if (jumpInput && grounded)
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

        //Dash

        //Somersault
        if (somersaultInput && grounded)
        {
            anim.SetTrigger("Somersault");
            StartCoroutine(AllowFalling());
        }

        //backflip
        if (backflipInput && grounded)
        {
            anim.SetTrigger("Backflip");
            StartCoroutine(AllowFalling());
        }

        //Slide
        if (dashInput && grounded)
        {
            anim.SetBool("Dash", true);
            StartCoroutine(StopDash());
        }
    }

    private IEnumerator StopDash()
    {
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("Dash", false);
    }

    private IEnumerator AllowFalling()
    {
        yield return new WaitForSeconds(2);
        ignoreFalling = false;
    }
    #endregion
}