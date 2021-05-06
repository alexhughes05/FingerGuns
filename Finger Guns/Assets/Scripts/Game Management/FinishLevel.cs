using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    #region Variables
    //public
    [SerializeField] int timeBonusMultiplier = 10;

    //private
    private Level level;
    private TimerCountdown timer;
    private GameSession session;
    #endregion
    void Awake()
    {
        level = FindObjectOfType<Level>();
        timer = FindObjectOfType<TimerCountdown>();
        session = FindObjectOfType<GameSession>();
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        level.EnterShop();

        int timeBonus = (int)timer.timeLeft * timeBonusMultiplier;
        session.AddToScore(timeBonus);
    }
}