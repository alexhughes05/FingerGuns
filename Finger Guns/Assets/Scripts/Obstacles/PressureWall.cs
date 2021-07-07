using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureWall : MonoBehaviour
{
    //public variables
    [SerializeField] float speedOfWall;
    [SerializeField] float timeBtwDamageTicks;

    //private variables
    private BoxCollider2D col;
    private PlayerHealth playerHealth;
    private Coroutine co;
    private bool playerDead;
    private bool coroutineStarted;

    private void Awake() 
    {
        col = GetComponent<BoxCollider2D>();
        playerHealth = FindObjectOfType<PlayerHealth>();
    } 

    // Update is called once per frame
    void Update() => transform.position += Vector3.right * Time.deltaTime * speedOfWall * 2;

    public float GetPressureWallXPos() => transform.position.x + col.size.x + 20; //The x position of the pressure wall
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
            if (playerHealth.Health <= 0)
            {
                playerDead = true;
                yield break;
            }
        }
    }
    //Properties
    #region Properties
    public float SpeedOfWall { get { return speedOfWall; } set {speedOfWall = value; } }
    #endregion
}
