using Unity.Netcode;
using UnityEngine;

public class NetworkStarter : MonoBehaviour
{
    public GameObject playerPrefab;

    public void StartHost()
    {
        NetworkManager.Singleton.NetworkConfig.PlayerPrefab = null; // Disable auto-spawn
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.NetworkConfig.PlayerPrefab = null; // Disable auto-spawn
        NetworkManager.Singleton.StartClient();
    }
}
