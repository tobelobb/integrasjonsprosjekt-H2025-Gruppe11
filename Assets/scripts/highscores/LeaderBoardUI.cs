using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    public GameObject rowPrefab;
    public Transform contentHolder;
    public LeaderboardManager leaderboardManager;

    public void PopulateLeaderboardUI(string gamemode)
    {
        Debug.Log("PopulateLeaderboardUI called with gamemode: " + gamemode);

        if (leaderboardManager == null)
        {
            Debug.LogError("LeaderboardManager reference not assigned!");
            return;
        }

        if (rowPrefab == null || contentHolder == null)
        {
            Debug.LogError("RowPrefab or ContentHolder not assigned!");
            return;
        }

        leaderboardManager.GetLeaderboardEntries(gamemode, (entries) =>
        {
            Debug.Log("Entries received: " + (entries?.Count ?? -1));

            if (entries == null || entries.Count == 0)
            {
                Debug.LogWarning("No leaderboard entries to display.");
                return;
            }

            foreach (Transform child in contentHolder)
                Destroy(child.gameObject);

            int rank = 1;
            foreach (var entry in entries)
            {
                Debug.Log($"Instantiating row for {entry.uid} with score {entry.score}");

                var row = Instantiate(rowPrefab, contentHolder);
                var texts = row.GetComponentsInChildren<TMP_Text>();

                if (texts.Length < 3)
                {
                    Debug.LogWarning("Row prefab does not contain enough TMP_Text components.");
                    continue;
                }

                texts[0].text = rank.ToString();       // RankText
                texts[1].text = entry.uid;             // NameText (or UID)
                texts[2].text = entry.score.ToString();// ScoreText
                rank++;
            }
        });
    }
}
