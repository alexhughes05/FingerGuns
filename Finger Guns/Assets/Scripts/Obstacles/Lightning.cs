using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    [HideInInspector]
    public Animator controller;
    [SerializeField] int minSpawnRateInSeconds;
    [SerializeField] int maxSpawnRateInSeconds;
    [SerializeField] float moveSpeed;
    [SerializeField] int numTimesExecPerSpawn;
    [SerializeField] float durationBtwEachAnim;
    private bool hasFinished;

    private void Start()
    {
        controller = GetComponent<Animator>();
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
            var targetPosition = new Vector3(GameObject.FindGameObjectsWithTag("Player")[0].transform.position.x, Camera.main.ViewportToWorldPoint(new Vector3(0, 0.7f, 5)).y, -5);
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            if (transform.position.x == targetPosition.x)
            {
                hasFinished = true;
                controller.SetTrigger("Lightning Strike");
                controller.SetBool("Lightning Strike", true);
            }
        }
        else
        {
            Destroy(gameObject, 2f);
        }
    }
}
