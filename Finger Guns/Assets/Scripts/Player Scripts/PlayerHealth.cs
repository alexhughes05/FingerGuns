using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    #region Variables
    //Public
    [SerializeField] int health;
    [SerializeField] Sprite fullHeart;
    [SerializeField] Sprite emptyHeart;
    [SerializeField] Image[] hearts;

    //Private
    private Level level;
    private int currentHealth;
    private bool deathTriggered;
    #endregion

    #region Monobehaviour Callbacks

    private void Awake()
    {
        level = FindObjectOfType<Level>();
    }
    void Start()
    {
        currentHealth = health;
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
        if (currentHealth <= 0 && !deathTriggered)
        {
            deathTriggered = true;
            GetComponent<FingerGunMan>().Anim.SetBool("Death", true);
            GetComponent<FingerGunMan>().PlayerDead = true;
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