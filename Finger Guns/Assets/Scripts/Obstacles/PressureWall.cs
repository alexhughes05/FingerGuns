using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureWall : MonoBehaviour
{
    //public variables
    [Range(0, 100)]
    [SerializeField] public int initialDistanceBehindPlayer;
    [SerializeField] public float speedOfWall;
    [SerializeField] public float timeBtwDamageTicks;
    [SerializeField] public ParticleSystem ps;

    //private variables
    private float startingCamXPos;
    private Camera cam;
    private Transform blackGradient;
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
        cam = Camera.main;
        col = GetComponent<BoxCollider2D>();
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector2(cam.transform.position.x - initialDistanceBehindPlayer, cam.transform.position.y);
        startingCamXPos = cam.transform.position.x;
        shape = ps.shape;  //Alows you to modify the boundaries of the particle system
        blackGradient = GameObject.Find("BlackGrad").transform;
    }

    public float GetPressureWallXPos()
    {
        return (transform.position.x + size); //The x position of the pressure wall is the current position + the size (however much it has expanded)
    }

    // Update is called once per frame
    void Update()
    {
        size += (speedOfWall * Time.deltaTime); //The wall is expanded based on the speed. A higher speed will expand it faster
        offset = size * 0.5f;  //In order to keep the collider bounded on the left, the offset has to be half the amount of the size. This will allow it to only expand in the right direction.
        col.offset = new Vector2(offset, col.offset.y); //set the offset of the collider
        col.size = new Vector2(size, col.size.y);  //set the size of the collider
        //particle effect
        shape.position = new Vector2(offset * 2f, cam.transform.position.y);  //To match the rate of the wall, the particle effect must expand at a rate of 2 * the collider offset

        if (blackGradient != null)
        {
            blackGradient.position = new Vector3(shape.position.x + transform.position.x - 7, cam.transform.position.y, 0);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerDead && !coroutineStarted)
        {
            coroutineStarted = true;
            co = StartCoroutine(HurtPlayer());  //Do damage to the player when you are inside the pressure wall
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerDead)
        {
            StopCoroutine(co);  //Stop doing damage once out of the pressure wall
            coroutineStarted = false;
        }
    }

    private IEnumerator HurtPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBtwDamageTicks); //How often the player should be damaged
            playerHealth.ModifyHealth(-1);
            if (playerHealth.GetHealth() <= 0)
            {
                playerDead = true;
                yield break;
            }
        }
    }
}
