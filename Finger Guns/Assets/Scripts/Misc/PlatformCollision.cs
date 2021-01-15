using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    Rigidbody2D myRigidBody;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // If player is moving up, ignore collisions between player and platforms
        if (myRigidBody.velocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(8, 10, true);
        }
        //else the collision will not be ignored
        else
        {
            Physics2D.IgnoreLayerCollision(8, 10, false);
        }
    }
}
