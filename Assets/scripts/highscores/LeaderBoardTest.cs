using UnityEngine;

public class LeaderboardTester : MonoBehaviour
{
    public LeaderboardManager leaderboardManager;

    void Start()
    {
        if (leaderboardManager == null)
            leaderboardManager = GetComponent<LeaderboardManager>();

        RunTests();
    }

    void RunTests()
    {
        Debug.Log("Starting leaderboard tests...");

        leaderboardManager.RoundFinish("1", "COOP", 500);
        leaderboardManager.RoundFinish("test_uid_2", "COOP", 300);
        leaderboardManager.RoundFinish("test_uid_1", "COOP", 450); // lower than personal best
        leaderboardManager.RoundFinish("test_uid_3", "COOP", 600);
        leaderboardManager.RoundFinish("test_uid_2", "COOP", 350); // higher than previous

        leaderboardManager.RoundFinish("test_uid_1", "Singleplayer", 500);
        leaderboardManager.RoundFinish("test_uid_2", "Singleplayer", 300);
        leaderboardManager.RoundFinish("test_uid_1", "Singleplayer", 450); // lower than personal best
        leaderboardManager.RoundFinish("test_uid_3", "Singleplayer", 600);
        leaderboardManager.RoundFinish("test_uid_2", "Singleplayer", 350); // higher than previous
    }
}
