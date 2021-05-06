using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variables
    //Components
    CapsuleCollider2D bulletCollider;
    Rigidbody2D rb2d;

    //Public
    public int damage = 5;
    public float speed = 500f;
    public float range = 2f;
    public bool homingShot;
    //public float homingSpeed = 10f;
    public bool blastShot;

    public GameObject blastExplosion;

    //Private
    GameObject enemy;
    private GameObject[] enemies;
    [HideInInspector]
    public Transform closestEnemy;

    private Vector3 enemyTarget;    
    private bool blastCollision = false;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<CapsuleCollider2D>();

        rb2d.velocity = transform.right * speed;

        if(blastShot)
        {
            StartCoroutine(DestroyBlastBullet());            
        }
        else
        {
            Destroy(gameObject, range);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(homingShot)
        {
            if (bulletCollider.IsTouchingLayers(12))
            {
                collision.gameObject.GetComponent<EnemyHealth>().ModifyHealth(-damage);
                Destroy(gameObject);                
            }
        }
        else
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
                collision.gameObject.GetComponent<EnemyHealth>().ModifyHealth(-damage);
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
    #endregion
}