using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    #region Variables

    //Public
    [Header("Enemy")]
    [SerializeField] int enemyPointValue = 100;
    public int health;

    //Components
    private GameSession gameSession;

    //Private    
    private int currentHealth;
    private bool isDead = false;
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
            GetComponent<AIPatrol>().Anim.SetTrigger("Death");
            Destroy(gameObject, 0.5f);
            AddPoints();
            isDead = true;
        }
        else
        {
            GetComponent<AIPatrol>().Anim.SetTrigger("Take Damage");
        }
    }

    public void AddPoints()
    {
        if(!isDead)
            gameSession.AddToScore(enemyPointValue);
    }

    public int GetHealth()
    {
        return currentHealth;
    }
    #endregion
}