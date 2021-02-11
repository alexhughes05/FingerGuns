using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShoot : MonoBehaviour
{
    #region Variables
    //Components
    PlayerMovement playerMovement;

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
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if(shooting && !playerMovement.playerDead)
        {
            Shoot();
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {        
        if(collision.gameObject.layer == 10)
        {
            firePoint.Rotate(collision.transform.position);
            //If player is not dead, enemy can shoot
            if(!collision.gameObject.GetComponent<PlayerMovement>().playerDead)
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