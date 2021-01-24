using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] bool looping = false;
    [SerializeField] GameObject[] obstacles;
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
    private IEnumerator SpawnBlades(GameObject obstacle, Blade blade)
    {
        blade.selectedPath = blade.getPaths()[UnityEngine.Random.Range(0, blade.getPaths().Length)];
        var startingSpawn = blade.selectedPath.transform.GetChild(0);
        Instantiate(obstacle, startingSpawn.position, Quaternion.identity);
        yield return new WaitForSeconds(UnityEngine.Random.Range(blade.getMinSpawnRateInSeconds(), blade.getMaxSpawnRateInSeconds()));
    }
    private IEnumerator SpawnLightning(GameObject obstacle, Lightning lightning)
    {
        yield return new WaitForSeconds(5);
    }
}
