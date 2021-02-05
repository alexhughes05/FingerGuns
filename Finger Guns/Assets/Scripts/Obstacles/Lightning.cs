using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    [HideInInspector]
    public Animator controller;
    [SerializeField] float timeBeforeStrike = 0.3f;
    [SerializeField] int minSpawnRateInSeconds;
    [SerializeField] int maxSpawnRateInSeconds;
    [SerializeField] float moveSpeed;
    [SerializeField] int numTimesExecPerSpawn;
    [SerializeField] float durationBtwEachAnim;
    [SerializeField] GameObject player;
    PlayerMovement playerScript;
    private bool hasFinished;

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
    }
}
