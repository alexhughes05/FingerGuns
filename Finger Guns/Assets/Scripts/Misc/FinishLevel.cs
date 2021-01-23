using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    Level level;
    void Awake()
    {
        level = FindObjectOfType<Level>();
    }
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        level.EnterShop();
    }
}
