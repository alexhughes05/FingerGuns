using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    bool tookHit = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var damageDealer = collision.collider.gameObject;
        if (damageDealer.CompareTag("Player"))
        {
            if (damageDealer && tookHit == false)
            {
                damageDealer.GetComponentInParent<PlayerHealth>().ModifyHealth(-damage);
                tookHit = true;
            }
        }
    }
}
