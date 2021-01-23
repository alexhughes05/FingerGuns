using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Level : MonoBehaviour
{
    GameSession gameSession;
    private void Awake()
    {
        gameSession = FindObjectOfType<GameSession>();
    }
    [SerializeField] int delayAfterDeath = 2;
    public void LoadStartMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void FirstLevel()
    {
        if (gameSession)
            gameSession.ResetGame();
        SceneManager.LoadScene(1);
    }

    public void NextLevel()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(levelIndex + 1);
    }

    public void EnterShop()
    {
        SceneManager.LoadScene("Shop");
    }

    public void DeathScreen()
    {
        StartCoroutine(DelayNextScene());
    }

    private IEnumerator DelayNextScene()
    {
        yield return new WaitForSeconds(delayAfterDeath);
        SceneManager.LoadScene("Death");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
