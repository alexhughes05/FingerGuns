using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    [HideInInspector]
    public Animator controller;

    public GameObject currentHitObject;
    public float circleRadius;
    public float maxDistance;
    public LayerMask layerMask;
    [HideInInspector]
    public bool lightningHit = false;
    [SerializeField] float timeBeforeStrike = 0.3f;
    [SerializeField] int minSpawnRateInSeconds;
    [SerializeField] int maxSpawnRateInSeconds;
    [SerializeField] float moveSpeed;
    [SerializeField] int numTimesExecPerSpawn;
    [SerializeField] float durationBtwEachAnim;
    [SerializeField] GameObject player;
    PlayerMovement playerScript;
    private bool hasFinished;

    private Vector2 origin;
    private Vector2 direction;
    private float currentHitDistance;

    private void Start()
    {
        controller = GetComponent<Animator>();
        playerScript = FindObjectOfType<PlayerMovement>();
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

    public int getNumTimesExecPerSpawn()
    {
        return numTimesExecPerSpawn;
    }
    private void MoveCloud()
    {
        if (!hasFinished)
        {
            var targetPosition = playerScript.gameObject.transform.position;
            origin = targetPosition;
            targetPosition.y = targetPosition.y + 1;
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
            Destroy(gameObject, 2f);
        }
    }

    private IEnumerator WaitBeforeLightning()
    {
        yield return new WaitForSeconds(timeBeforeStrike);
        controller.SetTrigger("Lightning Strike");
        RaycastHit2D hit;
        direction = Vector2.down;
        if (hit = Physics2D.CircleCast(origin, circleRadius, direction, maxDistance, layerMask))
        {
            currentHitObject = hit.transform.gameObject;
            currentHitObject.GetComponent<PlayerHealth>().ModifyHealth(-1);
            currentHitObject.GetComponent<Animator>().SetTrigger("Take Damage Electric");
            lightningHit = true;
            currentHitObject.GetComponent<PlayerMovement>().InitializeHitVariables();
            StartCoroutine(currentHitObject.GetComponent<PlayerMovement>().WaitAndAllowMovement());
            currentHitObject.GetComponent<PlayerMovement>().ConfigureShooting();
        }
    }
}
