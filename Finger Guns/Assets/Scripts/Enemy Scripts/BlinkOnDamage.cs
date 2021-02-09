using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkOnDamage : MonoBehaviour
{
    Health health;
    private Material matDamage;
    private Material matDefault;
    SkinnedMeshRenderer mr;
    // Start is called before the first frame update
    void Start()
    {
        health = gameObject.GetComponent<Health>();
        mr = gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        matDamage = Resources.Load("GhostyBoiDamaged", typeof(Material)) as Material;
        matDefault = mr.material;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            mr.material = matDamage;
        }


        if (health.GetHealth() > 0)
        {
            Invoke("ResetMaterial", .1f);
        }
    }

    void ResetMaterial()
    {
        mr.material = matDefault;
    }
}
