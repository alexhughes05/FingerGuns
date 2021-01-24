using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    #region Variables
    [Header("Enemy")]
    [SerializeField] int enemyPointValue = 80;
    //Public
    public int health;
    //public int maxHealth;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Image[] hearts;
    private GameSession gameSession;
    private Level level;

    //Private
    private int currentHealth;
    private bool addedScore;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {

        currentHealth = health;
        gameSession = FindObjectOfType<GameSession>();
        level = FindObjectOfType<Level>();
    }

    private void Update()
    {
        //Set up player health display
        if (fullHeart != null)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (i < currentHealth)
                    hearts[i].sprite = fullHeart;
                else
                    hearts[i].sprite = emptyHeart;

                if (i < health)
                    hearts[i].enabled = true;
                else
                    hearts[i].enabled = false;              
            }
        }
    }
    #endregion

    #region Private Methods
    public void ModifyHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth <= 0)
        {
            if(gameObject.tag == "Player")
            {
                GetComponent<PlayerMovement>().anim.SetTrigger("Death");
                GetComponent<PlayerMovement>().playerDead = true;
                Destroy(gameObject, 1f);
                level.DeathScreen();
            }
            else if (gameObject.tag == "Enemy" && addedScore == false)
            {
                addedScore = true;
                GetComponent<AIPatrol>().anim.SetTrigger("Death");
                Destroy(gameObject, 0.5f);
                gameSession.AddToScore(enemyPointValue);
            }            
        }
        else
        {
            if (gameObject.tag == "Enemy")
            {
                GetComponent<AIPatrol>().anim.SetTrigger("Take Damage");
            }
        }
    }
    #endregion
}