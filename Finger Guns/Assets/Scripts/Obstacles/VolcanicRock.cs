using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicRock : MonoBehaviour
{
    //public
    [SerializeField] float minRotationSpeed;
    [SerializeField] float maxRotationSpeed;

    //Components
    private PlayerHealth health;
    private Rigidbody2D rb2d;
    private bool firstTime = true;
    private bool startRotation;
    private float rotationSpeed;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        health = FindObjectOfType<PlayerHealth>();
    }

    private void Update()
    {
        if (StartRotation)
        {
            if (firstTime)
            {
                firstTime = false;
                rotationSpeed = UnityEngine.Random.Range(minRotationSpeed, maxRotationSpeed);
                rotationSpeed *= Random.Range(0, 2) * 2 - 1; //is either -1 or 1. Used to pick the direction to rotate
            }
            rb2d.rotation += rotationSpeed  * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            health.ModifyHealth(-1);
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
            Destroy(gameObject);
    }

    public bool StartRotation { get; set; }
}
