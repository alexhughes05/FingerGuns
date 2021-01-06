using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostyBoiAnimationTest : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            animator.SetFloat("Movement", -1.0f);
            if (Input.GetKeyDown("d"))
            {
                animator.SetFloat("Movement", 0.0f);
            }
        } else
        if (Input.GetKeyDown("d"))
        {
            animator.SetFloat("Movement", 1.0f);
        }
        if (Input.GetKeyDown("f"))
        {
            animator.SetFloat("Movement", 0.0f);
        }

        if (Input.GetKeyDown("w"))
        {
            animator.SetBool("Blink", true);
        }
        if (Input.GetKeyDown("s"))
        {
            animator.SetBool("Blink", false);
        }

        if (Input.GetKeyDown("q"))
        {
            animator.SetBool("Mouth Open", false);
        }
        if (Input.GetKeyDown("e"))
        {
            animator.SetBool("Mouth Open", true);
        }

        if (Input.GetKeyDown("z"))
        {
            animator.SetTrigger("Death");
        }
    }
}
