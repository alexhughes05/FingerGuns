using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerExplodey : MonoBehaviour
{
    //public
    [SerializeField] GameObject explodey;
    [SerializeField] GameObject fingerGunMan;

    //private
    private GameObject spawnedEnemy;
    private bool alreadyExecuted = false;
    private List<Vector2> spawnPoints = new List<Vector2>();

    private void Awake()
    {
        fingerGunMan.GetComponent<FingerGunMan>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10 && !alreadyExecuted)
        {
            alreadyExecuted = true;
            SetupSpawnPoints();
            SpawnExplodeyOne(); //Spawn explodeyone
            spawnedEnemy.GetComponent<ExplodeyOne>().MoveTowardsPlayer = true;
        }
    }
    private void SetupSpawnPoints()
    {
        Vector2 xSpawnPos = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width + 200, 0)); //max width explodeyone spawns from
        Vector2 ySpawnPos = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height + 200)); //max height explodeyone spawns from
        var minYSpawnPos = Mathf.RoundToInt(fingerGunMan.transform.position.y);
        var maxYSpawnPos = Mathf.RoundToInt(ySpawnPos.y);
        var minXSpawnPos = Mathf.RoundToInt(Camera.main.ScreenToWorldPoint(new Vector2(Screen.width / 2, 0)).x);
        var maxXSpawnPos = Mathf.RoundToInt(xSpawnPos.x);
        var xInterval = (maxXSpawnPos - minXSpawnPos) / 4;
        var yInterval = (maxYSpawnPos - minYSpawnPos) / 3;
        for (int y = minYSpawnPos; y <= maxYSpawnPos; y += yInterval)
            spawnPoints.Add(new Vector2(maxXSpawnPos, y));

        for (int x = minXSpawnPos; x <= maxXSpawnPos; x += xInterval)
            spawnPoints.Add(new Vector2(x, maxYSpawnPos));
    }

    private void SpawnExplodeyOne()
    {
        var selectedIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
        spawnedEnemy = Instantiate(explodey, spawnPoints[selectedIndex], Quaternion.identity);
    }
}
