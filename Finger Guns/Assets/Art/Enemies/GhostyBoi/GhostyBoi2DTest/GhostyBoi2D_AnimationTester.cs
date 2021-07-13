using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostyBoi2D_AnimationTester : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown("u"))
        {
            animator.SetFloat("Movement", -1.0f);
        }
        else if (Input.GetKeyDown("i"))
        {
            animator.SetFloat("Movement", 1.0f);
        }

        if (Input.GetKeyDown("j"))
        {
            animator.SetBool("Blinking", true);
        }
        else if (Input.GetKeyDown("k"))
        {
            animator.SetBool("Blinking", false);
        }

        if (Input.GetKeyDown("n"))
        {
            animator.SetBool("Mouth Open", true);
        }
        else if (Input.GetKeyDown("m"))
        {
            animator.SetBool("Mouth Open", false);
        }
    }
}
