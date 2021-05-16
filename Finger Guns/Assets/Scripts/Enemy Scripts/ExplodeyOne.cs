using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeyOne : MonoBehaviour
{
    [SerializeField] FingerGunMan player;
    [SerializeField] CircleCollider2D explosionAreaCollider;
    [SerializeField] float moveSpeed;

    //Components
    private Animator anim;
    private ParticleSystem explosion;
    private FingerGunMan playerScript;

    //Private
    private float playerXPos;
    private Vector2 targetPos;
    private bool inExplosionRadius;
    private bool deathAnimStarted;
    private bool explosionAnimStarted;

    private void Awake()
    {
        playerScript = FindObjectOfType<FingerGunMan>();
        explosion = GetComponentInChildren<ParticleSystem>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MoveTowardsPlayer)
        {
            playerXPos = playerScript.gameObject.transform.position.x;
            targetPos = new Vector2(playerXPos, playerScript.gameObject.transform.position.y);
            var movementThisFrame = moveSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, movementThisFrame);
            anim.SetFloat("Movement", moveSpeed);
        }

        if (anim.GetCurrentAnimatorStateInfo(2).IsName("Rig _ExplodeyOne|Death"))
        {
            if (anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0 && !deathAnimStarted)
            {
                deathAnimStarted = true;
                anim.SetTrigger("Eyes X");
            }
            if (!explosionAnimStarted && anim.GetCurrentAnimatorStateInfo(2).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(2).normalizedTime < 1)
            {
                explosionAnimStarted = true;
                GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                explosion.Play();
                if (inExplosionRadius)
                    player.GetComponent<PlayerHealth>().ModifyHealth(-1);
                Destroy(gameObject, 0.7f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsTouching(explosionAreaCollider))
        {
            if (collision.gameObject.layer == 10)
            {
                Debug.Log("executed death");
                inExplosionRadius = true;
                anim.SetTrigger("Death");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
        {
            inExplosionRadius = false;
        }
    }
    
    //Properties
    public bool MoveTowardsPlayer { get; set; }
}
