﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variables
    //Public
    [SerializeField] int damage = 2;
    [SerializeField] float speed = 500f;
    [SerializeField] float range = 0.3f;

    //Components
    private CapsuleCollider2D bulletCollider;
    private Rigidbody2D rb2d;

    //private
    private bool alreadyCollided;
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

        if (collision.gameObject.layer == 12 && !alreadyCollided)
        {
            alreadyCollided = true; //Needed becomes sometimes it registers as a collision twice
            var currentGameObjectTransform = collision.gameObject.transform;
            EnemyHealth enemyHealth;
            //Since the modifyHealth script isn't always on the same gameobject as the collider, keep checking its parent till you find it
            while ((enemyHealth = currentGameObjectTransform.gameObject.GetComponent<EnemyHealth>()) == null)
                currentGameObjectTransform = currentGameObjectTransform.transform.parent;
            enemyHealth.ModifyHealth(-damage);
            Destroy(gameObject); //Destroy the bullet
        }
    }
    #endregion
}