using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShoot : MonoBehaviour
{
    #region Variables
    //Components
    FingerGunMan fingerGunMan;

    //Public
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject enemyProjectile;
    [SerializeField] float timeBtwShots;

    //Private
    private float currentTimeBtwShots;
    #endregion

    #region Properties
    public bool Shooting { get; set; }
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        fingerGunMan = GameObject.FindGameObjectWithTag("Player").GetComponent<FingerGunMan>();
    }
    void Start()
    {
        currentTimeBtwShots = timeBtwShots;
    }

    void Update()
    {
        if(Shooting && !fingerGunMan.PlayerDead)
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
            if (!collision.gameObject.GetComponentInParent<FingerGunMan>().PlayerDead && fingerGunMan.ExternalForce == false)
                Shooting = true;
            else
                Shooting = false;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Shooting = false;
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