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
    [SerializeField] int damage = 5;
    [SerializeField] float speed = 500f;
    [SerializeField] float range = 2f;
    #endregion

    #region Monobehaviour Callbacks

    private void Awake()
    {
        rb2d = gameObject.GetComponent<Rigidbody2D>();
        bulletCollider = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        rb2d.velocity = transform.right * speed;
        Destroy(gameObject, range);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            collision.gameObject.GetComponent<EnemyHealth>().ModifyHealth(-damage);
            Destroy(gameObject);
        }
    }
    #endregion
}