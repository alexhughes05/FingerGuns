using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDebris : MonoBehaviour
{
    #region Variables
    //public
    [SerializeField] GameObject[] fallingDebris;
    [SerializeField] float minTimeBtwSpawns;
    [SerializeField] float maxTimeBtwSpawns;
    [SerializeField] float minDebrisSize;
    [SerializeField] float maxDebrisSize;
    [SerializeField] float fallingRate;

    //Components
    private SmokeAndRocksEffect smokeAndRocksEffect;
    private StartEruption startEruption;
    private GameObject spawnedObject;
    #endregion

    private void Awake()
    {
        smokeAndRocksEffect = FindObjectOfType<SmokeAndRocksEffect>();
        startEruption = FindObjectOfType<StartEruption>();
    }

    public void StartRainingDebris()
    {
        StartCoroutine(RainDebrisForDuration());
    }

    private IEnumerator RainDebrisForDuration()
    {
        Coroutine co = StartCoroutine(SpawnDebris());
        yield return new WaitForSeconds(smokeAndRocksEffect.RocksDuration - startEruption.DelayToStartRainingDebris);
        StopCoroutine(co);
    }
    private IEnumerator SpawnDebris()
    {
        while (true)
        {
            GameObject selectedDebris = fallingDebris[UnityEngine.Random.Range(0, fallingDebris.Length)];
            float sizeMultiplier = UnityEngine.Random.Range(minDebrisSize, maxDebrisSize);
            selectedDebris.transform.localScale = Vector3.one * 100f;
            selectedDebris.transform.localScale *=  sizeMultiplier;
            Vector2 stageDimensions = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            Vector2 stageLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            Vector2 spawnLocation = new Vector2(UnityEngine.Random.Range(stageLeft.x + 1, stageDimensions.x - 1), stageDimensions.y + 1);
            spawnedObject = Instantiate(selectedDebris, spawnLocation, transform.rotation);
            spawnedObject.GetComponent<VolcanicRock>().StartRotation = true;
            spawnedObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -fallingRate);
            yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeBtwSpawns, maxTimeBtwSpawns));
        }
    }
}
