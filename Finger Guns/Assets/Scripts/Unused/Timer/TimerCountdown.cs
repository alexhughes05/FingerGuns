using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerCountdown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] int startTime = 30;
    [SerializeField] bool takingAway = false;

    [HideInInspector] public float timeLeft;
    private float currentTime;
    private string timerString;
    private int currentSeconds;
    private int currentMinutes;

    void Start()
    {
        currentTime = startTime;

        currentSeconds = (int)(currentTime % 60);
        currentMinutes = (int)(currentTime / 60) % 60;

        timerString = string.Format("{0:00}:{1:00}", currentMinutes, currentSeconds);

        textDisplay.text = timerString;
    }

    void Update()
    {
        if(takingAway == false && currentTime >= 0)
        {
            StartCoroutine(DecreaseTime());
        }        

        currentSeconds = (int)(currentTime % 60);
        currentMinutes = (int)(currentTime / 60) % 60;

        timeLeft = currentTime;

        timerString = string.Format("{0:00}:{1:00}", currentMinutes, currentSeconds);

        if (currentTime <= 0)
            FindObjectOfType<Level>().DeathScreen();
    }

    IEnumerator DecreaseTime()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        currentTime -= 1;

        textDisplay.text = timerString;
        takingAway = false;
    }
}