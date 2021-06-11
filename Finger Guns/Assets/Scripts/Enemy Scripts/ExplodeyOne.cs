using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeyOne : MonoBehaviour
{
    //public
    [SerializeField] CircleCollider2D explosionAreaCollider;
    [SerializeField] float moveSpeed;
    [HideInInspector]
    [SerializeField] bool lockOntoPlayer;
    [HideInInspector]
    [SerializeField] float explosionDelay;

    //Components
    private Animator anim;
    private ParticleSystem explosion;
    private FingerGunMan playerScript;

    //Private
    private Vector2 playerCurrentPos;
    private Vector2 playerPosOnTrigger;
    private bool origPlayerPosReceived;
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
            if (lockOntoPlayer)
            {
                playerCurrentPos = playerScript.gameObject.transform.position;
                TriggerMoveTowards(playerCurrentPos);
            }
            else
            {
                if (!origPlayerPosReceived)
                {
                    origPlayerPosReceived = true;
                    playerPosOnTrigger = playerScript.gameObject.transform.position;
                }
                TriggerMoveTowards(playerPosOnTrigger);

                if (Vector2.Distance(playerPosOnTrigger, transform.position) <= 0.1)
                    StartCoroutine(DelayBeforeDetonating());
            }
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
                    playerScript.gameObject.GetComponent<PlayerHealth>().ModifyHealth(-1);
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

    private void TriggerMoveTowards(Vector2 destinationPos)
    {
        var movementThisFrame = moveSpeed * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, destinationPos, movementThisFrame);
        anim.SetFloat("Movement", moveSpeed);
    }

    private IEnumerator DelayBeforeDetonating()
    {
        yield return new WaitForSeconds(explosionDelay);
        anim.SetTrigger("Death");
    }

    //Properties
    public bool MoveTowardsPlayer { get; set; }
}
