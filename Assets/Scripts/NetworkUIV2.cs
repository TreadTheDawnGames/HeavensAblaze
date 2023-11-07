using FishNet.Managing;
using FishNet.Transporting.UTP;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.WebRequestMethods;
using TMPro;
using UnityEditor;

public class NetworkUIV2 : MonoBehaviour
{
    [SerializeField] private NetworkManager _networkManager;

    string joinCode = "";
    FishyUnityTransport utp;

    bool serverStarted = false;
    bool clientStarted = false;

    [SerializeField]
    TMP_Text serverButtonText;

    [SerializeField]
    TMP_Text clientButtonText;

    [SerializeField]
    TMP_InputField joinCodeBox;

    
    
    private async void Start()
    {
        utp = (FishyUnityTransport)_networkManager.TransportManager.Transport;
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
            {
              Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        serverButtonText.text = "Starting\nServer";
        clientButtonText.text = "Starting\nClient";

        if (!serverStarted)
        {

            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(9);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                utp.SetRelayServerData(new RelayServerData(allocation, "dtls"));

                Debug.Log(joinCode);
                joinCodeBox.text = joinCode;
                joinCodeBox.interactable = false;
                this.joinCode = joinCode;

            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
            }

        // Start Server Connection
        _networkManager.ServerManager.StartConnection();
        // Start Client Connection
        _networkManager.ClientManager.StartConnection();

            serverButtonText.text = "Stop\nServer";
            clientButtonText.text = "Stop\nClient";

        }
        else
        {
        _networkManager.ClientManager.StopConnection();
        _networkManager.ServerManager.StopConnection(true);
            serverButtonText.text = "Start\nServer";
            clientButtonText.text = "Start\nclient";
            joinCodeBox.interactable = true;

        }



        serverStarted = !serverStarted;
        clientStarted = !clientStarted;
        if (serverStarted)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

        }
    }

    public async void JoinRelay()
    {
        clientButtonText.text = "Starting\nClient";

        if (!clientStarted)
        {

            if (joinCode.Length != 6)
            {
                Debug.LogWarning("No joinCode provided, or code is invalid! Code:" + joinCode);
                return;
            }

            try
            {
                Debug.Log("Joining Relay with " + joinCode);
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                if (!serverStarted){

                utp.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
                }

            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _networkManager.ClientManager.StartConnection();

            clientButtonText.text = "Stop\nClient";
        }
        else
        {
        _networkManager.ClientManager.StopConnection();
            clientButtonText.text = "Start\nClient";

        }
        clientStarted = !clientStarted;

        
    }

    public void SyncJoinCode(string s)
    {
        joinCode = s;
        
    }



}