using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variables
    public int damage = 5;
    public float speed = 500f;
    public float range = 2f;
    public bool homingShot;
    public float homingSpeed = 10f;
    public bool blastShot;

    public GameObject blastExplosion;
    GameObject enemy;

    private GameObject[] enemies;
    [HideInInspector]
    public Transform closestEnemy;

    private Vector3 enemyTarget;
    private Rigidbody2D rb2d;
    private bool blastCollision = false;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        rb2d.velocity = transform.right * speed;

        closestEnemy = null;

        if(blastShot)
        {
            StartCoroutine(DestroyBlastBullet());            
        }
        else
        {
            Destroy(gameObject, range);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {       
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Health>().ModifyHealth(-damage);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Special conditions for homing shot, because of it's "homing" circle collider
        //All other bullets, make sure they don't collide with enemy circle detection collider
        if (collision.GetType() != typeof(CircleCollider2D) && !homingShot)
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

            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Health>().ModifyHealth(-damage);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger != true && collision.CompareTag("Enemy"))
        {
            if (homingShot)
            {
                closestEnemy = GetClosestEnemy();
                Vector3 targetPosition = closestEnemy.position - gameObject.transform.position;
                rb2d.AddForce(targetPosition * homingSpeed);
                transform.LookAt(closestEnemy);
            }
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