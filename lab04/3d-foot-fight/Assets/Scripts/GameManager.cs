using Agents;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameSettings initialGameSettings;

    public int currentLives;
    public int currentScore;

    [SerializeField] private Damageable playerDamageable;
    [SerializeField] private Character playerCharacter;

    private Vector3 initialPlayerPosition;

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

    private void Start()
    {
        playerDamageable.SetHealth(initialGameSettings.initialHealth, initialGameSettings.initialHealth);
        initialPlayerPosition = playerCharacter.transform.position;
    }

    private void InitializeGameState()
    {
        currentLives = initialGameSettings.initialLives;
    }

    public void OnPlayerScored()
    {
        currentScore++;
        if (currentScore >= initialGameSettings.scoreGoal) EndGame(true);
    }

    public void OnPlayerLostLife()
    {
        Debug.Log("OnPlayerLostLife");
        currentLives--;
        Debug.Log(currentLives);

        if (currentLives <= 0)
        {
            EndGame(false);
            return;
        }

        Respawn();
    }

    private void Respawn()
    {
        var controller = playerCharacter.GetComponent<CharacterController>();

        controller.enabled = false;
        playerCharacter.transform.position = initialPlayerPosition;
        controller.enabled = true;

        playerDamageable.SetHealth(initialGameSettings.initialHealth, initialGameSettings.initialHealth);
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