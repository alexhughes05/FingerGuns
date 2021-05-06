using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureWall : MonoBehaviour
{
    //public variables
    [SerializeField] public float speedOfWall;
    [SerializeField] public float timeBtwDamageTicks;
    [SerializeField] public ParticleSystem ps;

    //private variables
    private BoxCollider2D col;
    private PlayerHealth playerHealth;
    private float offset;
    private float size;
    private Coroutine co;
    private bool playerDead;
    private bool coroutineStarted;
    private ParticleSystem.ShapeModule shape;

    private void Awake()
    {        
        col = GetComponent<BoxCollider2D>();
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        shape = ps.shape;
    }

    public float GetPressureWallXPos()
    {
        return (transform.position.x + size);
    }

    // Update is called once per frame
    void Update()
    {
        size += (speedOfWall * Time.deltaTime);
        offset = size * 0.5f;
        col.offset = new Vector2(offset, col.offset.y);
        col.size = new Vector2(size, col.size.y);
        col.size = new Vector2(size, col.size.y);
        //particle effect
        shape.position = new Vector2(offset * 2f, shape.position.y);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerDead && !coroutineStarted)
        {
            coroutineStarted = true;
            co = StartCoroutine(HurtPlayer());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerDead)
        {
            StopCoroutine(co);
            coroutineStarted = false;
        }
    }

    private IEnumerator HurtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBtwDamageTicks);
            playerHealth.ModifyHealth(-1);
            if (playerHealth.GetHealth() <= 0)
            {
                playerDead = true;
                yield break;
            }
        }
    }
}
