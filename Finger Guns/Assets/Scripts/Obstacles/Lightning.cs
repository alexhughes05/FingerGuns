using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    #region Variables
    //Public
    [SerializeField] float circleRadius;
    [SerializeField] float maxDistance;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float timeBeforeStrike = 0.3f;
    [SerializeField] int minSpawnRateInSeconds;
    [SerializeField] int maxSpawnRateInSeconds;
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject player;
    
    //Components
    private FingerGunMan playerScript;
    private Wind wind;

    //Private
    private GameObject currentHitObject;
    private bool hasFinished;
    private Vector2 origin;
    private Vector2 direction;
    private bool isMovingRight;
    private float defaultMoveSpeed;
    #endregion

    private void Awake()
    {
        wind = FindObjectOfType<Wind>();
        Controller = GetComponent<Animator>();
        playerScript = FindObjectOfType<FingerGunMan>();
    }

    private void Start()
    {
        defaultMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCloud();
    }
    public int getMinSpawnRateInSeconds()
    {
        return minSpawnRateInSeconds;
    }
    public int getMaxSpawnRateInSeconds()
    {
        return maxSpawnRateInSeconds;
    }

    private void MoveCloud()
    {
        moveSpeed = defaultMoveSpeed; //Set the move speed back to default so it doesn't keep constantly increasing or dercreasing it
        if (!hasFinished)
        {
            var targetPosition = playerScript.gameObject.transform.position;
            origin = targetPosition;

            if (wind.WindActive)
            {
                if (playerScript.playerXMovement > 0)
                    isMovingRight = true;

                if (playerScript.playerXMovement == 0) //If wind is blowing and player is not moving, cloud moves slightly faster than the new player speed
                    moveSpeed += Mathf.Abs(wind.currentWindForce);
                else if ((isMovingRight && wind.currentWindForce < 0) || (!isMovingRight && wind.currentWindForce > 0)) //If wind is opposing your movement, cloud goes slower
                    moveSpeed = Mathf.Abs(moveSpeed - (moveSpeed - playerScript.defaultMaxSpeed) * 2 - wind.currentWindForce);
                else if ((isMovingRight && wind.currentWindForce > 0) || (!isMovingRight && wind.currentWindForce < 0)) //If wind in the same direction as your movement, cloud goes faster.
                    moveSpeed += Mathf.Abs(wind.currentWindForce);
            }

            targetPosition.y++;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position.x == targetPosition.x)
            {
                hasFinished = true;
                StartCoroutine(WaitBeforeLightning());
            }
        }
        else
        {
            Destroy(gameObject, 1.5f);
        }
    }

    private IEnumerator WaitBeforeLightning()
    {
        yield return new WaitForSeconds(timeBeforeStrike);
        Controller.SetTrigger("Lightning Strike");
        RaycastHit2D hit;
        direction = Vector2.down;
        if (hit = Physics2D.CircleCast(origin, circleRadius, direction, maxDistance, layerMask))
        {
            currentHitObject = hit.transform.gameObject;
            currentHitObject.GetComponent<PlayerHealth>().ModifyHealth(-1);
            currentHitObject.GetComponent<Animator>().SetTrigger("Take Damage Electric");
            currentHitObject.GetComponent<FingerGunMan>().ShootingEnabled = false;
            currentHitObject.GetComponent<FingerGunMan>().ExternalForce = true;
            currentHitObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            StartCoroutine(currentHitObject.GetComponent<FingerGunMan>().WaitToMove());
        }
    }

    #region Properties
    //Properties
    public Animator Controller { get; set; }

    public bool LightningHit { get; set; } = false;
    #endregion
}
