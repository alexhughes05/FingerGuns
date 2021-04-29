using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] bool looping = false;
    [SerializeField] GameObject[] obstacles;
    private Blade blade;
    private Lightning lightning;
    
    IEnumerator Start()
    {
        do
        {
            //UnityEngine.Random.Range(0, obstacles.Length)
            GameObject obstacle = obstacles[0];
            if (obstacle)
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
        while (looping && (FindObjectOfType<PlayerHealth>()?.GetHealth() > 0));
    }

    private IEnumerator SpawnBlades(GameObject obstacle, Blade blade)
    {
        blade.selectedPath = blade.getPaths()[UnityEngine.Random.Range(0, blade.getPaths().Length)];
        var spawnHeight = blade.selectedPath.transform.GetChild(0).position.y;

        var spawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.5f, 5));
        spawnPoint.y = spawnHeight;
        Instantiate(obstacle, spawnPoint, Quaternion.identity);

        yield return new WaitForSeconds(UnityEngine.Random.Range(blade.getMinSpawnRateInSeconds(), blade.getMaxSpawnRateInSeconds()));
    }
    private IEnumerator SpawnLightning(GameObject obstacle, Lightning lightning)
    {
        int randomNum = UnityEngine.Random.Range(0, 2);
        if (randomNum == 0)
        {
            Instantiate(obstacle, Camera.main.ViewportToWorldPoint(new Vector3(0, 0.7f, 5)), Quaternion.identity);
        }
        else if (randomNum == 1)
        {
            Instantiate(obstacle, Camera.main.ViewportToWorldPoint(new Vector3(1, 0.7f, 5)), Quaternion.identity);
        }
        yield return new WaitForSeconds(UnityEngine.Random.Range(lightning.getMinSpawnRateInSeconds(), lightning.getMaxSpawnRateInSeconds()));
    }
}
