using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    #region Variables
    //Public
    public float speed;

    //Private
    private Transform player;
    private Vector2 target;
    private FingerGunMan fingerGunMan;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        fingerGunMan = FindObjectOfType<FingerGunMan>();
        if (!fingerGunMan.playerDead)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform; //bug where player can't be found after death
            target = new Vector2(player.position.x, player.position.y + 1);

            //Point towards target position
            Vector3 relativePos = player.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            rotation.x = transform.rotation.x;
            rotation.y = transform.rotation.y;
            transform.rotation = rotation;
        }
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if(transform.position.x == target.x && transform.position.y == target.y)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag != "Enemy" &&
            collision.gameObject.layer != 9)
            Destroy(gameObject);

        if (collision.gameObject.layer == 10 && fingerGunMan.externalForce == false)
        {
            collision.gameObject.GetComponentInParent<Animator>().SetTrigger("Take Damage");
            collision.GetComponentInParent<PlayerHealth>().ModifyHealth(-1);
        }
    }
    #endregion
}
