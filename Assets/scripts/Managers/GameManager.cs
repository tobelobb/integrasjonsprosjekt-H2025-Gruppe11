using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;   // NEW: Leaderboards
using Unity.Services.Core;           // NEW: Services init
using Unity.Services.Authentication; // NEW: Authentication

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;         // GameOverPanel
    public TextMeshProUGUI finalScoreText;   // FinalScoreText
    public GameObject pausePanel;            // PauseMenuPanel

    public bool IsGameOver { get; private set; }
    private bool isPaused;

    private int score;

    private const string LEADERBOARD_ID = "global_highscore"; // NEW

    async void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        UpdateScoreUI();

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);

        IsGameOver = false;
        isPaused = false;

        // Initialize Unity Services + sign in
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Services init failed: {e.Message}");
        }
    }

    void Update() { }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
    }

    public async void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        // Show UI
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = $"Score: {score}";

        // Save score locally
        int currentBest = PlayerPrefs.GetInt("bestScore", 0);
        if (score > currentBest)
        {
            PlayerPrefs.SetInt("bestScore", score);
            PlayerPrefs.Save();
        }

        // Submit score to global leaderboard
        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(LEADERBOARD_ID, score);
            Debug.Log($"Submitted {score} to {LEADERBOARD_ID}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Leaderboard submission failed: {e.Message}");
        }

        // Stop player control
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            var move = player.GetComponent<PlayerMovement>();
            var shoot = player.GetComponent<PlayerShooting>();
            if (move) move.enabled = false;
            if (shoot) shoot.enabled = false;
        }

        foreach (var sp in Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
            sp.StopSpawning();

        foreach (var e in Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None))
            e.enabled = false;
    }

    public void TogglePause()
    {
        if (IsGameOver) return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (pausePanel) pausePanel.SetActive(isPaused);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel) pausePanel.SetActive(false);
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f; // reset in case paused
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // reset in case paused
        SceneManager.LoadScene("MainMenuScene");
    }
}
