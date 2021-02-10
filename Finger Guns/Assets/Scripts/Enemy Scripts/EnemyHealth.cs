using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    #region Variables
    //Components
    private GameSession gameSession;

    [Header("Enemy")]
    //Public
    [SerializeField] int enemyPointValue = 100;    
    public int health;

    //Private    
    private int currentHealth;
    private bool addedScore;
    #endregion

    #region Monobehaviour Callbacks
    void Start()
    {
        currentHealth = health;
        gameSession = FindObjectOfType<GameSession>();
    }
    #endregion

    #region Private Methods
    public void ModifyHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth <= 0)
        {
            addedScore = true;
            GetComponent<AIPatrol>().anim.SetTrigger("Death");
            Destroy(gameObject, 0.5f);
            gameSession.AddToScore(enemyPointValue);        
        }
        else
        {
            GetComponent<AIPatrol>().anim.SetTrigger("Take Damage");
        }
    }

    public int GetHealth()
    {
        return currentHealth;
    }
    #endregion
}