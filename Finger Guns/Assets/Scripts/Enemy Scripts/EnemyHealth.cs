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
    private Animator anim;

    //Private    
    private int currentHealth;
    private bool isDead = false;
    #endregion

    #region Monobehaviour Callbacks

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        gameSession = FindObjectOfType<GameSession>();
    }
    void Start()
    {
        currentHealth = health;
    }
    #endregion

    #region Private Methods
    public void ModifyHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth <= 0)
        {
            anim.SetTrigger("Death");
            if (gameObject.name.Contains("ExplodeyOne"))
            {
                Destroy(gameObject, 1);
            }
            else
                Destroy(gameObject, 0.5f);
            AddPoints();
            isDead = true;
        }
        else
            anim.SetTrigger("Take Damage");
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