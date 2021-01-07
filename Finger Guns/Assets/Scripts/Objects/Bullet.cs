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
    public bool blastShot;

    public GameObject blastExplosion;

    private GameObject[] enemies;
    [HideInInspector]
    public Transform closestEnemy;
    [HideInInspector]
    public bool enemyContact;

    private Vector3 enemyTarget;
    private Rigidbody rb;
    private bool blastCollision = false;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = transform.right * speed;

        closestEnemy = null;
        enemyContact = false;

        if(blastShot)
        {
            StartCoroutine(DestroyBlastBullet());            
        }
        else
        {
            Destroy(gameObject, range);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (blastShot)
        {
            blastCollision = true;

            StartCoroutine(DestroyBlastBullet());
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
    IEnumerator DestroyBlastBullet()
    {        
        if(blastCollision)
            yield return new WaitForSeconds(0);
        else
            yield return new WaitForSeconds(range);

        GameObject explosion = Instantiate(blastExplosion,
                transform.position, Quaternion.identity);
        Destroy(explosion, 1f);
        Destroy(gameObject);
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
