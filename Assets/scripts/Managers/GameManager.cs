using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject gameOverPanel;         // assign your GameOverPanel
    public TextMeshProUGUI finalScoreText;   // assign FinalScoreText inside the panel

    public bool IsGameOver { get; private set; }

    private int score;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        UpdateScoreUI();
        if (gameOverPanel) gameOverPanel.SetActive(false);
        IsGameOver = false;
    }

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

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        // Show UI
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (finalScoreText) finalScoreText.text = $"Score: {score}";

        // Stop player control
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            var move = player.GetComponent<PlayerMovement>();
            var shoot = player.GetComponent<PlayerShooting>();
            if (move) move.enabled = false;
            if (shoot) shoot.enabled = false;
        }

        // Stop spawners
        foreach (var sp in FindObjectsOfType<EnemySpawner>())
            sp.enabled = false;

        // Optionally stop existing enemies from moving
        foreach (var e in FindObjectsOfType<Enemy>())
            e.enabled = false; // keeps sprite visible
    }

    // Hook this to the Restart button OnClick
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
