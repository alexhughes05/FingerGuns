using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    bool tookHit = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var fingerGunMan = collision.collider.GetComponent<Health>();
        if (fingerGunMan && tookHit == false)
        {
            fingerGunMan.modifyHealth(damage);
            tookHit = true;
        }
    }
}
