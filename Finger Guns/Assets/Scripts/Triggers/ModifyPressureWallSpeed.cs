using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyPressureWallSpeed : MonoBehaviour
{
    //public
    [SerializeField] float newWallSpeed;
    //Components
    private PressureWall pw;

    private void Awake()
    {
        pw = FindObjectOfType<PressureWall>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10)
            pw.SpeedOfWall = newWallSpeed;
    }
}
