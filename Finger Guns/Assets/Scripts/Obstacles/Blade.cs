using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    #region Variables
    //public
    [SerializeField] int minSpawnRateInSeconds;
    [SerializeField] int maxSpawnRateInSeconds;
    [SerializeField] float speedOfSpin;
    [SerializeField] float moveSpeed;
    [SerializeField] GameObject[] pathsPrefab;
    
    //Components
    private Rigidbody2D rgbd;
    #endregion

    private void Awake()
    {
        rgbd = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        MoveBlade();
    }

    public int getMinSpawnRateInSeconds()
    {
        return minSpawnRateInSeconds;
    }
    public int getMaxSpawnRateInSeconds()
    {
        return maxSpawnRateInSeconds;
    }

    public GameObject[] getPaths()
    {
        return pathsPrefab;
    }

    private void MoveBlade()
    {
        rgbd.velocity = new Vector2(-moveSpeed, 0);
        rgbd.rotation += speedOfSpin * Time.deltaTime;
        var destroyPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        if (transform.position.x <= destroyPos.x && !IsStationary)
        {
            Destroy(gameObject);
        }
    }

    //Properties
    public bool IsStationary { get; set; }

    public GameObject SelectedPath { get; set; }
}

