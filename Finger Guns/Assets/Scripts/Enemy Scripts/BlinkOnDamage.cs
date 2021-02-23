using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkOnDamage : MonoBehaviour
{
    [SerializeField] float blinkDuration = 0.05f;
    [SerializeField] float timeBetweenBlinks = 0.05f;
    EnemyHealth health;
    private Material matDamage;
    private Material matDefault;
    SkinnedMeshRenderer mr;
    
    void Start()
    {
        health = gameObject.GetComponent<EnemyHealth>();
        mr = gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        matDamage = Resources.Load("GhostyBoiDamaged", typeof(Material)) as Material;
        matDefault = mr.material;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            mr.material = matDamage;
            if (health.GetHealth() > 0)
            {
                Invoke("ResetMaterial", blinkDuration); 
            }
            StartCoroutine(BlinkTwice());
        }
    }

    IEnumerator BlinkTwice()
    {
        yield return new WaitForSeconds(blinkDuration + timeBetweenBlinks);
        mr.material = matDamage;
        if (health.GetHealth() > 0)
        {
            Invoke("ResetMaterial", blinkDuration); 
        }
    }

    void ResetMaterial()
    {
        mr.material = matDefault;
    }
}
