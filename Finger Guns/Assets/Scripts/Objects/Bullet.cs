using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variables
    public float damage = 5f;
    public float speed = 20f;
    public float range = 2f;
    public bool homingShot;

    public GameObject homingExplosion;

    private GameObject[] enemies;
    [HideInInspector]
    public Transform closestEnemy;
    [HideInInspector]
    public bool enemyContact;

    private Vector3 enemyTarget;
    private Rigidbody rb;
    private bool homingCollision = false;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = transform.right * speed;

        closestEnemy = null;
        enemyContact = false;

        if(homingShot)
        {
            StartCoroutine(DestroyHomingBullet());

            GameObject explosion = Instantiate(homingExplosion, 
                transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }
        else
        {
            Destroy(gameObject, range);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (homingShot)
        {
            homingCollision = true;
            StartCoroutine(DestroyHomingBullet());

            GameObject explosion = Instantiate(homingExplosion, 
                transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.isTrigger != true && collision.CompareTag("Enemy"))
        {
            if (homingShot)
            {
                closestEnemy = GetClosestEnemy();
                Vector3 targetPosition = closestEnemy.position - gameObject.transform.position;
                rb.AddForce(targetPosition * 10f);
                transform.LookAt(closestEnemy);
            }
            enemyContact = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.isTrigger != true && collision.CompareTag("Enemy"))
        {
            enemyContact = false;
        }
    }
    #endregion

    #region Private Methods
    IEnumerator DestroyHomingBullet()
    {        
        if(homingCollision)
        {
            Destroy(gameObject);
            yield return new WaitForSeconds(0f);            
        }
        else
        {
            Destroy(gameObject, range);
            yield return new WaitForSeconds(range);            
        }        
    }

    public Transform GetClosestEnemy()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform transform = null;

        foreach (GameObject enemy in enemies)
        {            
            float currentDistance;
            currentDistance = Vector3.Distance(gameObject.transform.position, enemy.transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                transform = enemy.transform;
            }
        }
        return transform;
    }
    #endregion
}
