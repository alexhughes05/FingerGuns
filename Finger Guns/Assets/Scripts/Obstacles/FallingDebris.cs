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
    [SerializeField] float durationOfDisaster;
    [SerializeField] float minDebrisSize;
    [SerializeField] float maxDebrisSize;
    [SerializeField] float fallingRate;
    #endregion

    public void StartRainingDebris()
    {
        StartCoroutine(RainDebrisForDuration());
    }

    private IEnumerator RainDebrisForDuration()
    {
        Coroutine co = StartCoroutine(SpawnDebris());
        yield return new WaitForSeconds(durationOfDisaster);
        StopCoroutine(co);
    }
    private IEnumerator SpawnDebris()
    {
        while (true)
        {
            GameObject selectedDebris = fallingDebris[UnityEngine.Random.Range(0, fallingDebris.Length)];
            float sizeMultiplier = UnityEngine.Random.Range(minDebrisSize, maxDebrisSize);
            selectedDebris.transform.localScale = Vector2.one * sizeMultiplier;
            Vector2 stageDimensions = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            Vector2 stageLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            Vector2 spawnLocation = new Vector2(UnityEngine.Random.Range(stageLeft.x + 1, stageDimensions.x - 1), stageDimensions.y + 1);
            GameObject spawnedObject = Instantiate(selectedDebris, spawnLocation, Quaternion.identity);
            spawnedObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -fallingRate);
            yield return new WaitForSeconds(UnityEngine.Random.Range(minTimeBtwSpawns, maxTimeBtwSpawns));
        }
    }
}
