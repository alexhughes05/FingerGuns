using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShoot : MonoBehaviour
{
    #region Variables
    //Components
    private CircleCollider2D detectionCollider;

    //Public
    public Transform firePoint;
    public GameObject enemyProjectile;
    public float timeBtwShots;

    //Private
    private float currentTimeBtwShots;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        detectionCollider = GetComponent<CircleCollider2D>();

        currentTimeBtwShots = timeBtwShots;
    }

    void Update()
    {
        
    }

    void OnTriggerStay2D(Collider2D collision)
    {        
        if(collision.gameObject.tag == "Player")
        {
            firePoint.Rotate(collision.transform.position);
            Shoot();
        }
    }
    #endregion

    #region Private Methods
    void Shoot()
    {
        if(currentTimeBtwShots <= 0)
        {
            Instantiate(enemyProjectile, firePoint.transform.position, Quaternion.identity);
            currentTimeBtwShots = timeBtwShots;
        }
        else
        {
            currentTimeBtwShots -= Time.deltaTime;
        }
    }
    #endregion
}
