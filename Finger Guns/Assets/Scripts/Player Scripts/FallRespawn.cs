using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallRespawn : MonoBehaviour
{
    [SerializeField] Transform spawnPoint1;
    [SerializeField] Transform spawnPoint2;

    public bool postTutorial = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 10)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;

            if (!postTutorial)
                player.position = (Vector2)spawnPoint1.position;
            else
                player.position = (Vector2)spawnPoint2.position;
        }
    }

    public void SetPostTutorial(bool state)
    {
        postTutorial = state;
    }
}