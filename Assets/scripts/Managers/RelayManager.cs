using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    private bool isHostStarted = false;
    private bool isClientStarted = false;

    public string CurrentJoinCode { get; private set; }

    public async Task<string> StartHostAsync()
    {
        if (isHostStarted) return CurrentJoinCode;
        isHostStarted = true;

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        CurrentJoinCode = joinCode;

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            allocation.RelayServer.IpV4,
            (ushort)allocation.RelayServer.Port,
            allocation.AllocationIdBytes,
            allocation.Key,
            allocation.ConnectionData
        );

        NetworkManager.Singleton.StartHost();
        return joinCode;
    }

    public async Task StartClientAsync(string joinCode)
    {
        if (isClientStarted) return;
        isClientStarted = true;

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(
            joinAllocation.RelayServer.IpV4,
            (ushort)joinAllocation.RelayServer.Port,
            joinAllocation.AllocationIdBytes,
            joinAllocation.Key,
            joinAllocation.ConnectionData,
            joinAllocation.HostConnectionData
        );

        NetworkManager.Singleton.StartClient();
    }
}
