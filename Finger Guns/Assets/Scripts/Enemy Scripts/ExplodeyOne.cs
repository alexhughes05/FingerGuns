using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeyOne : MonoBehaviour
{
    //Components
    private Animator anim;
    private ParticleSystem explosion;

    //Private
    private bool deathAnimStarted;
    private bool explosionAnimStarted;

    private void Awake()
    {
        explosion = GetComponentInChildren<ParticleSystem>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Rig _ExplodeyOne|Death"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && !deathAnimStarted)
            {
                deathAnimStarted = true;
                anim.SetTrigger("Eyes X");
            }
            if (!explosionAnimStarted && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                explosionAnimStarted = true;
                GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                explosion.Play();
            }
        }
    }
}
