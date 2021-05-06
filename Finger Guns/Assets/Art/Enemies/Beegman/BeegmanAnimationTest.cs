using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeegmanAnimationTest : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown("j"))
        {
            animator.SetFloat("Movement Speed", -1);
        }
        if (Input.GetKeyDown("l"))
        {
            animator.SetFloat("Movement Speed", 1);
            if (Input.GetKey("j"))
            {
                animator.SetFloat("Movement Speed", 0);
            }
        }

        if (Input.GetKeyDown("i"))
        {
            animator.SetBool("Blink", true);
        }
        if (Input.GetKeyDown("k"))
        {
            animator.SetBool("Blink", false);
        }

        if (Input.GetKeyDown("u"))
        {
            animator.SetTrigger("Take Damage");
        }
        if (Input.GetKeyDown("o"))
        {
            animator.SetTrigger("Death");
        }

        if (Input.GetKeyDown("n"))
        {
            animator.SetBool("Mouth Open", true);
        }
        if (Input.GetKeyDown("m"))
        {
            animator.SetBool("Mouth Open", false);
        }
    }
}
