using UnityEngine;
using TMPro;
using Unity.Services.Leaderboards;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI personalBestText;
    public TextMeshProUGUI globalLeaderboardText;
    public TextMeshProUGUI welcomeText; // Optional: shows "Welcome, [username]"

    private const string LEADERBOARD_ID = "global_highscore";

    async void Start()
    {
        // Show signed-in username
        if (AuthenticationService.Instance.IsSignedIn && welcomeText != null)
        {
            string playerName = AuthenticationService.Instance.PlayerName;
            welcomeText.text = $"Welcome, {playerName}";
        }

        // Load this account's best score from leaderboard
        await LoadPersonalBest();

        // Load global leaderboard
        await LoadGlobalLeaderboard();
    }

    /// Submit a score to the global leaderboard.
    public async Task SubmitScore(int score)
    {
        try
        {
            await LeaderboardsService.Instance.AddPlayerScoreAsync(LEADERBOARD_ID, score);
            Debug.Log($"Submitted {score} to {LEADERBOARD_ID}");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Leaderboard submission failed: {e.Message}");
        }
    }

    /// Load the signed-in player's best score from the leaderboard.
    private async Task LoadPersonalBest()
    {
        try
        {
            var playerEntry = await LeaderboardsService.Instance.GetPlayerScoreAsync(LEADERBOARD_ID);
            if (playerEntry != null && personalBestText != null)
            {
                personalBestText.text = $"Your Best: {playerEntry.Score}";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to load personal best: {e.Message}");
            if (personalBestText != null)
                personalBestText.text = "Your Best: N/A";
        }
    }

    /// Load the top scores from the global leaderboard.
    private async Task LoadGlobalLeaderboard()
    {
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(
                LEADERBOARD_ID,
                new GetScoresOptions { Limit = 10 }
            );

            string leaderboardDisplay = "<b>GLOBAL BEST</b>\n\n";
            foreach (var entry in scores.Results)
            {
                string name = string.IsNullOrEmpty(entry.PlayerName) ? "Anonymous" : entry.PlayerName;
                leaderboardDisplay += $"{entry.Rank + 1,2}. {name,-20} {entry.Score,5}\n";
            }

            if (globalLeaderboardText != null)
                globalLeaderboardText.text = leaderboardDisplay;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to load leaderboard: {e.Message}");
            if (globalLeaderboardText != null)
                globalLeaderboardText.text = "Global leaderboard unavailable";
        }
    }
}
