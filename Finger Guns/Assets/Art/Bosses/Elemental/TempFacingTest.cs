using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempFacingTest : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            animator.SetFloat("XMovement", -1.0f);
        }
        else if (Input.GetKeyDown("x"))
        {
            animator.SetFloat("XMovement", 1.0f);
        }
    }
}
