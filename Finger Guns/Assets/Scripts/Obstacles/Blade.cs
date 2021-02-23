﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    [SerializeField] int minSpawnRateInSeconds;
    [SerializeField] int maxSpawnRateInSeconds;
    [SerializeField] float speedOfSpin;
    [SerializeField] float moveSpeed;
    public bool isStationary;
    [SerializeField] GameObject[] pathsPrefab;
    [HideInInspector] public GameObject selectedPath;
    Rigidbody2D rgbd;

    public void Start()
    {
        rgbd = GetComponent<Rigidbody2D>();
    }

    public void Update()
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
        if (transform.position.x <= destroyPos.x && !isStationary)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10)
        {            
            collision.gameObject.GetComponentInParent<PlayerMovement>().bladeHit = true;
            if (!isStationary)
                Destroy(gameObject);
        }
    }
}
