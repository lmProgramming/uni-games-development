using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameSettings initialGameSettings;

    public int currentLives;
    public float currentHealth;
    public int currentScore;

    private void Awake()
    {
        if (initialGameSettings == null)
        {
            Debug.LogError("Didn't assign GameSettings to GameManager!", this);
            enabled = false;
            return;
        }

        InitializeGameState();
    }

    private void InitializeGameState()
    {
        currentLives = initialGameSettings.initialLives;
        currentHealth = initialGameSettings.initialHealth;
    }

    public void OnPlayerScored()
    {
        currentScore++;
        Debug.Log($"OnPlayerScored: {currentScore}");
        Debug.Log($"OnPlayerScored: {currentLives}");
        Debug.Log($"OnPlayerScored: {initialGameSettings.scoreGoal}");
        if (currentScore >= initialGameSettings.scoreGoal) EndGame(true);
    }

    public void OnPlayerLostLife()
    {
        currentLives--;
        if (currentLives <= 0) EndGame(false);
    }

    private static void EndGame(bool positive)
    {
        Debug.Log(positive ? "Nice! You win!" : "You lost!");
        SceneManager.LoadScene("main");
    }

    public int GetPointsLeft()
    {
        return initialGameSettings.scoreGoal - currentScore;
    }
}