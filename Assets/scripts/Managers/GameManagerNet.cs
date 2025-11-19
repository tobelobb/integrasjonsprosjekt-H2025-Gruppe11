using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;   // Leaderboards
using Unity.Services.Core;           // Services init
using Unity.Services.Authentication; // Authentication

public class GameManagerNet : NetworkBehaviour
{
    public static GameManagerNet Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;       // GameOverPanel
    public TextMeshProUGUI finalScoreText; // FinalScoreText
    public GameObject pausePanel;          // PauseMenuPanel

    private NetworkVariable<int> score = new NetworkVariable<int>(0);
    private bool isPaused;
    private bool isGameOver = false;

    private const string LEADERBOARD_ID = "global_highscore";

    async void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);

        isPaused = false;
        isGameOver = false;

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

        // Subscribe to score changes
        score.OnValueChanged += OnScoreChanged;
    }

    void OnDestroy()
    {
        score.OnValueChanged -= OnScoreChanged;
    }

    void OnScoreChanged(int oldValue, int newValue)
    {
        if (scoreText) scoreText.text = $"Score: {newValue}";
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(int amount)
    {
        if (isGameOver) return;
        score.Value += amount;
    }

    [ServerRpc(RequireOwnership = false)]
    public void GameOverServerRpc()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Broadcast GameOver to all clients
        ShowGameOverClientRpc();

        // Stop spawning enemies (server only)
        foreach (var sp in Object.FindObjectsByType<EnemySpawnerNet>(FindObjectsSortMode.None))
            sp.StopSpawning();

        // Submit score asynchronously (server only)
        SubmitScoreToLeaderboard(score.Value);
    }

    private async void SubmitScoreToLeaderboard(int finalScore)
    {
        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(LEADERBOARD_ID, finalScore);
            Debug.Log($"Submitted {finalScore} to {LEADERBOARD_ID}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Leaderboard submission failed: {e.Message}");
        }
    }

    [ClientRpc]
    void ShowGameOverClientRpc()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = $"Score: {score.Value}";
    }

    public void TogglePause()
    {
        if (isGameOver) return;

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
        if (IsServer)
        {
            Time.timeScale = 1f;
            NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }

    public void LoadMainMenu()
    {
        if (IsServer)
        {
            Time.timeScale = 1f;
            NetworkManager.Singleton.SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
        }
    }
}
