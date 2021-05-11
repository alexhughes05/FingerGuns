using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    #region Variables
    //Components
    private Level level;

    //Public
    public int health;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Image[] hearts;    

    //Private
    private int currentHealth;
    private bool addedScore;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        currentHealth = health;
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
            GetComponent<FingerGunMan>().anim.SetTrigger("Death");
            GetComponent<FingerGunMan>().playerDead = true;
            Destroy(gameObject, 1f);
            level.DeathScreen();      
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }
    #endregion
}