using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCollision : MonoBehaviour
{
    #region Variables
    //Components
    Rigidbody2D myRigidBody;

    //Public
    public Transform groundCheck;
    public LayerMask platformLayer;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //If player is moving up, ignore collisions between player and platforms
        if (myRigidBody.velocity.y > 0)
        {
            Physics2D.IgnoreLayerCollision(9, 10, true);
        }
        /* Else if player's feet don't touch a platform, ignore the collision
         * This enable the player to run through platforms 
         */
        else if (!Physics2D.OverlapCircle(groundCheck.position, 0.3f, platformLayer))
        {
            Physics2D.IgnoreLayerCollision(9, 10, true);
        }
        //Else the collision will not be ignored
        else
        {
            Physics2D.IgnoreLayerCollision(9, 10, false);
        }
    }
    #endregion
}