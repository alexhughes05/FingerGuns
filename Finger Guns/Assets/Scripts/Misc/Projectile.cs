using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    bool tookHit = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var damageDealer = collision.collider.GetComponent<Health>();
        if (damageDealer && tookHit == false)
        {
            damageDealer.ModifyHealth(-damage);
            tookHit = true;
        }
    }
}
