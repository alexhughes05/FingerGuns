using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    //Components
    private Rigidbody2D rb2d;
    [HideInInspector]
    public Animator anim;

    //Public Variables
    [Header("Movement")]
    public float doubleTapWindow = 0.5f;
    public float movementSpeed = 10f;
    public float jumpForce = 5f;
    public float somersaultForceX = 3f;
    public float somersaultForceY = 3f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public bool facingRight = true;
    [Space()]
    public float AFKTimer = 10f;

    [Header("Camera")]
    public Transform cameraTarget;
    public float moveAheadAmount, moveAheadSpeed;
    [Space()]

    [Header("Weapon")]
    public Transform firePoint;    

    //Other Private Variables        
    private bool grounded;
    private int buttonCount = 0;
    private float horizontalInput;
    private bool jumpInput;
    private bool dashInput;    
    private bool somersaultInput;
    private bool slideInput;
    private bool backflipInput;

    private float currentAFKTime;
    #endregion

    #region Monobehaviour Callbacks 
    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        currentAFKTime = AFKTimer;
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
        //Movement
        horizontalInput = Input.GetAxis("Horizontal");
        //Jump
        jumpInput = Input.GetButtonDown("Jump");
        //SomerSault
        somersaultInput = GetSomersaultInput();
        //Backflip
        //backflipInput = Input.GetKeyDown(KeyCode.LeftShift);
    }

    private bool GetSomersaultInput()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {

            if (doubleTapWindow > 0 && buttonCount == 1/*Number of Taps you want Minus One*/)
            {
                //Has double tapped
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
        grounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        //Movement
        rb2d.velocity = new Vector2(horizontalInput * movementSpeed, rb2d.velocity.y); //Might have to put Time.deltaTime
        if (horizontalInput < 0 && facingRight)
            Flip();
        else if (horizontalInput > 0 && !facingRight)
            Flip();

        //Move Camera Target
        if (horizontalInput != 0)
        {
            cameraTarget.localPosition = new Vector3(Mathf.Lerp(cameraTarget.localPosition.x, 
                moveAheadAmount, moveAheadSpeed * Time.deltaTime), 
                cameraTarget.localPosition.y, cameraTarget.localPosition.z);
        }

        //Jump
        if(jumpInput && grounded)
        {
            rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
        //SomerSault
        if (somersaultInput && grounded)
        {
            rb2d.AddForce(new Vector2(somersaultForceX, somersaultForceY), ForceMode2D.Impulse);
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
        {
            anim.SetTrigger("Jump");
        }

        //Dash

        //Somersault
        if (somersaultInput && grounded)
        {
            anim.SetTrigger("Somersault");
        }
        //Slide

        //Backflip

        //Death
    }
    #endregion
}