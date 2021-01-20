using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShoot : MonoBehaviour
{
    #region Variables
    //Public
    public Transform firePoint;
    public GameObject enemyProjectile;
    public float timeBtwShots;

    //Private
    private float currentTimeBtwShots;
    private bool shooting;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        currentTimeBtwShots = timeBtwShots;
    }

    void Update()
    {
        if(shooting)
        {
            Shoot();
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {        
        if(collision.gameObject.tag == "Player")
        {
            firePoint.Rotate(collision.transform.position);
            shooting = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        shooting = false;
    }
    #endregion

    #region Private Methods
    void Shoot()
    {        
        if (currentTimeBtwShots <= 0)
        {
            Instantiate(enemyProjectile, firePoint.position, Quaternion.identity);
            currentTimeBtwShots = timeBtwShots;
        }
        else
        {
            currentTimeBtwShots -= Time.deltaTime;
        }
    }
    #endregion
}