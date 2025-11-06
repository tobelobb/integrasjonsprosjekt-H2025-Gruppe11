using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class MultiplayerUI : MonoBehaviour
{
    public RelayManager relayManager;
    public Button hostButton;
    public Button joinButton;
    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeDisplay;
    public Button startButton;

    void Start()
    {
        hostButton.onClick.AddListener(async () => await HostGame());
        joinButton.onClick.AddListener(async () => await JoinGame());
    }

    async Task HostGame()
    {
        string joinCode = await relayManager.StartHostAsync();
        joinCodeDisplay.text = $"Join Code: {joinCode}";
    }

    async Task JoinGame()
    {
        string code = joinCodeInput.text.Trim();
        await relayManager.StartClientAsync(code);
    }

    public void HideUI()
    {
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        joinCodeInput.gameObject.SetActive(false);
        joinCodeDisplay.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);
    }
}
