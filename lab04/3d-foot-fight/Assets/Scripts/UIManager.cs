using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;

    [SerializeField] private TextMeshProUGUI prisonersLeftText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI livesLeftText;
    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    private void Update()
    {
        var prisonersLeft = gameManager.GetPointsLeft();
        prisonersLeftText.text = $"Poor prisoners left to kill: {prisonersLeft}";

        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        pauseMenu.SetActive(false);
    }

    public void ShowGameOver()
    {
        Time.timeScale = 0;
        gameOverMenu.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("main");
    }
}