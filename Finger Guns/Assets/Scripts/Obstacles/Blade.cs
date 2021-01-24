using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    [SerializeField] int minSpawnRateInSeconds;
    [SerializeField] int maxSpawnRateInSeconds;
    [SerializeField] float speedOfSpin;
    [SerializeField] float moveSpeed;
    [SerializeField] int numTimesExecPerSpawn;
    [SerializeField] float durationBtwEachAnim;
    [SerializeField] GameObject[] pathsPrefab;
    [HideInInspector] public GameObject selectedPath;
    int waypointIndex = 0;
    List<Transform> waypoints;

    public void Start()
    {
        waypoints = GetWaypoints(selectedPath);
    }

    private List<Transform> GetWaypoints(GameObject path)
    {
        var waypoints = new List<Transform>();
        foreach(Transform waypoint in path.transform)
        {
            waypoints.Add(waypoint);
        }
        return waypoints;
    }

    public void Update()
    {
        MoveBlade(waypoints);
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

    public GameObject[] getPaths()
    {
        return pathsPrefab;
    }

    private void MoveBlade(List<Transform> waypoints)
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            var targetPosition = waypoints[waypointIndex].transform.position;
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movementThisFrame);
            transform.Rotate(0, 0, (speedOfSpin / (1/moveSpeed)) * Time.deltaTime);
            if (transform.position.x == targetPosition.x)
            {
                waypointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
