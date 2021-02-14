using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    #region Variables
    //Components
    CapsuleCollider2D bulletCollider;
    Rigidbody2D rb2d;

    //Public
    public float homingSpeed = 10f;

    //Private
    GameObject enemy;
    private GameObject[] enemies;
    [HideInInspector] public Transform closestEnemy;    
    
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        rb2d = gameObject.GetComponentInParent<Rigidbody2D>();

        closestEnemy = null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger == false && collision.CompareTag("Enemy"))
        {
                closestEnemy = GetClosestEnemy();
                Vector3 targetPosition = (closestEnemy.position - gameObject.transform.position).normalized;
                //float rotateAmount = transform.InverseTransformDirection(Vector3.Cross(targetPosition, transform.forward)).z;

                //rb2d.angularVelocity = new Vector3(0, 0, rotateAmount);
                rb2d.AddForce(targetPosition * homingSpeed);
        }
    }
    #endregion

    #region Private Methods
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