using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLevel : MonoBehaviour
{
    Level level;
    TimerCountdown timer;
    GameSession session;

    public int timeBonusMultiplier = 10;

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