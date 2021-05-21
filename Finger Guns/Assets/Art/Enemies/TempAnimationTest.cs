using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempAnimationTest : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            animator.SetFloat("Movement", -1.0f);
        }
        if (Input.GetKeyDown("x"))
        {
            animator.SetFloat("Movement", 1.0f);
        }
    }
}
