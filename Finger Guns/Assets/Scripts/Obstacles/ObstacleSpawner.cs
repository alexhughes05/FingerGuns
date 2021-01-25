using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] bool looping = false;
    [SerializeField] GameObject[] obstacles;
    public Vector2 followOffset;
    Vector3 leftCloudSpawn;
    Vector3 rightCloudSpawn;
    Blade blade;
    Lightning lightning;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        do
        {
            foreach (GameObject obstacle in obstacles)
            {
                if (obstacle.CompareTag("Blade"))
                {
                    blade = obstacle.GetComponent<Blade>();
                    yield return StartCoroutine(SpawnBlades(obstacle, blade));
                }
                else if (obstacle.CompareTag("Lightning"))
                {
                    lightning = obstacle.GetComponent<Lightning>();
                    yield return StartCoroutine(SpawnLightning(obstacle, lightning));
                }
            }
        }
        while (looping);
    }

    private void Update()
    {
        leftCloudSpawn = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.7f, 5));
        rightCloudSpawn = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.7f, 5));

    }
    private IEnumerator SpawnBlades(GameObject obstacle, Blade blade)
    {
        blade.selectedPath = blade.getPaths()[UnityEngine.Random.Range(0, blade.getPaths().Length)];
        var startingSpawn = blade.selectedPath.transform.GetChild(0);
        Instantiate(obstacle, startingSpawn.position, Quaternion.identity);
        yield return new WaitForSeconds(UnityEngine.Random.Range(blade.getMinSpawnRateInSeconds(), blade.getMaxSpawnRateInSeconds()));
    }
    private IEnumerator SpawnLightning(GameObject obstacle, Lightning lightning)
    {
        int randomNum = UnityEngine.Random.Range(0, 2);
        if (randomNum == 0)
        {
            Instantiate(obstacle, leftCloudSpawn, Quaternion.identity);
        }
        else if (randomNum == 1)
        {
            Instantiate(obstacle, rightCloudSpawn, Quaternion.identity);
        }
        yield return new WaitForSeconds(UnityEngine.Random.Range(lightning.getMinSpawnRateInSeconds(), lightning.getMaxSpawnRateInSeconds()));
    }
}
