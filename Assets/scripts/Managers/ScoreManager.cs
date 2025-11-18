using UnityEngine;
using TMPro;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ScoreManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI personalBestText;

    private const string BEST_SCORE_KEY = "bestScore";

    void Start()
    {
        // Always load local best score
        int best = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        if (personalBestText != null)
            personalBestText.text = $"Your Best: {best}";

        // Optionally also try to load from Cloud Save
        LoadScores();
    }

    public async Task SaveBestScore(int score)
    {
        // Save locally
        int currentBest = PlayerPrefs.GetInt(BEST_SCORE_KEY, 0);
        if (score > currentBest)
        {
            PlayerPrefs.SetInt(BEST_SCORE_KEY, score);
            PlayerPrefs.Save();
        }

        // Save to Cloud Save (if available)
        try
        {
            var data = new Dictionary<string, object> { { BEST_SCORE_KEY, score } };
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        }
        catch
        {
            Debug.LogWarning("Cloud Save failed, but local save succeeded.");
        }
    }

    public async void LoadScores()
    {
        try
        {
            var results = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { BEST_SCORE_KEY });
            if (results.TryGetValue(BEST_SCORE_KEY, out var savedValue))
            {
                int cloudBest = int.Parse(savedValue.ToString());
                if (personalBestText != null)
                    personalBestText.text = $"Your Best: {cloudBest}";
            }
        }
        catch
        {
            Debug.LogWarning("Cloud Load failed, using local PlayerPrefs.");
        }
    }
}
