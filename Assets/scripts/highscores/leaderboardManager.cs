using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;
using System;


public class LeaderboardManager : MonoBehaviour
{
	public void RoundFinish(string userid, string gamemode, int score)
	{
        CheckIfHighscore(userid, gamemode, score, (isHighscore) =>
		{
			if (isHighscore)
			{
				UpdatePersonalScore(userid, gamemode, score);
				Debug.Log("New highscore! Personal score updated.");
				CheckIfLeaderboard(userid, gamemode, score, (mode) =>
				{
					if (mode > 0)
					{
						UpdateLeaderboard(userid, gamemode, score);
						Debug.Log("new leaderboard entry.");
					}
					else
					{
						Debug.Log("Not a new leaderboard highscore.");
					}
				});
			}
			else
			{
				Debug.Log("Not a new highscore.");
			}
		});
	}

	public void CheckIfHighscore(string userId, string gamemode, int newScore, System.Action<bool> callback)
{
    var root = FirebaseDatabase.DefaultInstance.RootReference;
    var userScoreRef = root.Child("users").Child(userId).Child("highscores").Child(gamemode);

    userScoreRef.GetValueAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Error reading personal best: " + task.Exception);
            callback(false);
            return;
        }

        int oldScore = 0;
        if (task.Result.Exists)
        {
            oldScore = int.Parse(task.Result.Value.ToString());
        }

        bool isNewHighscore = newScore > oldScore;
        callback(isNewHighscore);
    });
}

	public void UpdatePersonalScore(string userId, string gamemode, int newScore)
	{
		var root = FirebaseDatabase.DefaultInstance.RootReference;

		// Update user profile
		var userScoreRef = root.Child("users").Child(userId).Child("highscores").Child(gamemode);
		userScoreRef.SetValueAsync(newScore);
	}

    public void UpdateLeaderboard(string userId, string gamemode, int newScore)
    {
        var leaderboardRef = FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("leaderboards")
            .Child(gamemode);

        CheckIfLeaderboard(userId, gamemode, newScore, (mode) =>
        {
            if (mode == 1)
            {
                // Leaderboard full, remove lowest and add new
                leaderboardRef.OrderByValue().LimitToFirst(1).GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (!task.IsFaulted && task.Result.Exists)
                    {
                        foreach (var child in task.Result.Children)
                        {
                            leaderboardRef.Child(child.Key).RemoveValueAsync();
                            Debug.Log("Removed lowest leaderboard entry: " + child.Key);
                        }
                        leaderboardRef.Child(userId).SetValueAsync(newScore);
                        Debug.Log("Inserted new highscore.");
                    }
                });
            }
            else if (mode == 2 || mode == 3)
            {
                leaderboardRef.Child(userId).SetValueAsync(newScore);
                Debug.Log("Leaderboard entry added or updated.");
            }
            else
            {
                Debug.Log("No leaderboard update needed.");
            }
        });
    }


    // returns 0 if no leaderboard entry
    // returns 1 if its a new record
    //returns 2 if its replacing own record
    //returns 3 if the highscore is not yet full
    public void CheckIfLeaderboard(string userId, string gamemode, int newScore, System.Action<int> callback)
	{
		var leaderboardRef = FirebaseDatabase.DefaultInstance
			.RootReference
			.Child("leaderboards")
			.Child(gamemode);
		var maxEntries = 100; //potentially change with gamemode or league

		// Query top scores (ascending, so lowest is first)
		Query topScoresQuery = leaderboardRef.OrderByValue().LimitToFirst(maxEntries);

		topScoresQuery.GetValueAsync().ContinueWithOnMainThread(task =>
		{
			if (task.IsFaulted)
			{
				Debug.LogError("Error reading leaderboard: " + task.Exception);
				callback(0);
				return;
			}

			DataSnapshot snapshot = task.Result;

			// Case 1: User already on leaderboard
			if (snapshot.HasChild(userId))
			{
				int oldScore = int.Parse(snapshot.Child(userId).Value.ToString());
				if (newScore > oldScore)
				{
					Debug.Log("user has older worse score.");
					callback(2);
				}
				else
				{
					Debug.Log("No update needed, old score is higher or equal.");
					callback(0);
				}
				return;
			}

			// Case 2: Leaderboard not full yet
			if (snapshot.ChildrenCount < maxEntries)
			{
				Debug.Log("leaderboard not full free entry.");
				callback(3);
				return;
			}

			// Case 3: Leaderboard full, check lowest directly
			DataSnapshot lowestEntry = null;
			int lowestScore = int.MaxValue;

			foreach (var child in snapshot.Children)
			{
				int score = int.Parse(child.Value.ToString());
				if (score < lowestScore)
				{
					lowestScore = score;
					lowestEntry = child;
				}
			}

			// Abort early if new score isn’t better
			if (newScore > lowestScore)
			{
				Debug.Log("Score getting on high enough.");
				callback(1);
				return;
			}
			else
			{
				Debug.Log("Score not high enough for leaderboard.");
				callback(0);
			}
		});
	}

    public void GetLeaderboardEntries(string gamemode, System.Action<List<(string uid, int score)>> callback)
    {
        var leaderboardRef = FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("leaderboards")
            .Child(gamemode);

        var maxEntries = 100;

        leaderboardRef.OrderByValue().LimitToLast(maxEntries).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to load leaderboard: " + task.Exception);
                callback(null);
                return;
            }

            if (!task.Result.Exists)
            {
                Debug.LogWarning("Leaderboard exists but has no entries.");
                callback(new List<(string uid, int score)>());
                return;
            }

            var entries = new List<(string uid, int score)>();
            foreach (var child in task.Result.Children)
            {
                var value = child.Value;
                int score;

                if (value is long)
                    score = Convert.ToInt32((long)value);
                else if (int.TryParse(value.ToString(), out score))
                    score = score;
                else
                {
                    Debug.LogWarning("Could not parse score for " + child.Key);
                    continue;
                }

                entries.Add((child.Key, score));
            }

            entries.Sort((a, b) => b.score.CompareTo(a.score));
            callback(entries);
        });
    }

}
