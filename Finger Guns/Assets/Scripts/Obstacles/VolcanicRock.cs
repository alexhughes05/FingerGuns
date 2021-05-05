using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicRock : MonoBehaviour
{
    //public
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;

    //Components
    private PlayerHealth health;

    private void Awake()
    {
        health = FindObjectOfType<PlayerHealth>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Physics2D.OverlapCircle(groundCheck.position, 0, groundLayer))
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            health.ModifyHealth(-1);
            Destroy(gameObject);
        }
    }
}
