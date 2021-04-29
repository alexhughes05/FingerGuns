using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkOnDamage : MonoBehaviour
{
    //public
    [SerializeField] float blinkDuration = 0.05f;
    [SerializeField] float timeBetweenBlinks = 0.05f;

    //components
    private EnemyHealth health;
    private SkinnedMeshRenderer mr;
    
    //private
    private Material matDamage;
    private Material matDefault;

    private void Awake()
    {
        health = gameObject.GetComponent<EnemyHealth>();
        mr = gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
    }

    void Start()
    {
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
