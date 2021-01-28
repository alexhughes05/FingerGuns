using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThundercloudAnimationTester : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown("a")){
            animator.SetTrigger("Lightning Strike");
        }
    }
}
