using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class MultiplayerUI : MonoBehaviour
{
    public RelayManager relayManager;
    public EnemySpawnerNet spawner;   // reference to the networked spawner
    public Button hostButton;
    public Button joinButton;
    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeDisplay;
    public Button startButton;

    void Start()
    {
        // Wire up button events
        hostButton.onClick.AddListener(async () => await HostGame());
        joinButton.onClick.AddListener(async () => await JoinGame());
        startButton.onClick.AddListener(StartGame); // hook up Start button
    }

    async Task HostGame()
    {
        string joinCode = await relayManager.StartHostAsync();
        joinCodeDisplay.text = $"{joinCode}";
    }

    async Task JoinGame()
    {
        string code = joinCodeInput.text.Trim();
        await relayManager.StartClientAsync(code);
    }

    void StartGame()
    {
        // Tell the server to begin spawning enemies
        if (spawner != null)
        {
            spawner.BeginSpawningServerRpc();
        }
        else
        {
            Debug.LogWarning("EnemySpawnerNet reference not set in MultiplayerUI!");
        }

        // Hide lobby UI
        HideUI();
    }

    public void HideUI()
    {
        if (hostButton) hostButton.gameObject.SetActive(false);
        if (joinButton) joinButton.gameObject.SetActive(false);
        if (joinCodeInput) joinCodeInput.gameObject.SetActive(false);
        if (joinCodeDisplay) joinCodeDisplay.gameObject.SetActive(false);
        if (startButton) startButton.gameObject.SetActive(false);
    }
}
