using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartEruption : MonoBehaviour
{
    //Components
    private FallingDebris debris;

    private void Awake()
    {
        debris = FindObjectOfType<FallingDebris>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            debris.StartRainingDebris();
    }
}
