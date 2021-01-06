using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    //Components
    private Rigidbody rb;

    //Public Variables
    [Header("Movement")]
    public float movementSpeed = 10f;
    public float jumpForce = 5f;
    [Space()]

    [Header("Camera")]
    public Transform cameraTarget;
    public float moveAheadAmount, moveAheadSpeed;

    //Other Private Variables
    private float horizontalInput;
    private bool jumpInput;
    #endregion

    #region Monobehaviour Callbacks 
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        GetInput();
        PerformMovement();
    }
    #endregion

    #region Private Methods
    private void GetInput()
    {
        //Movement
        horizontalInput = Input.GetAxis("Horizontal");
        //Jump
        jumpInput = Input.GetButtonDown("Jump");
    }

    private void PerformMovement()
    {
        //Movement
        transform.position += new Vector3(horizontalInput, 0, 0) * 
            Time.deltaTime * movementSpeed;
        //Move Camera Target
        if(horizontalInput != 0)
        {
            cameraTarget.localPosition = new Vector3(Mathf.Lerp(cameraTarget.localPosition.x, 
                moveAheadAmount * horizontalInput, moveAheadSpeed * Time.deltaTime), 
                cameraTarget.localPosition.y, cameraTarget.localPosition.z);
        }

        //Jump
        if(jumpInput && Mathf.Abs(rb.velocity.y) < 0.001f)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode.Impulse);
        }
    }
    #endregion
}
