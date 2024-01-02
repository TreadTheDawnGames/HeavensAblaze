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
using FishNet.Managing.Client;
using FishNet.Transporting;
using FishNet.Demo.AdditiveScenes;
using UnityEngine.Video;
using System.IO;

public class NetworkUIV2 : MonoBehaviour
{
    [SerializeField] private NetworkManager _networkManager;

    string joinCode = "";
    FishyUnityTransport utp;

    bool serverStarted = false;
    public bool clientStarted { get; private set; } = false;

    [SerializeField]
    TMP_Text serverButtonText;

    [SerializeField]
    TMP_Text clientButtonText;

    [SerializeField]
    TMP_InputField joinCodeBox;

    [SerializeField]
    VideoPlayer menuVideoPlayer;

    public InputManager inputManager;

    [SerializeField]
    GameObject optionsMenu;

    [SerializeField]
    AudioSource ambientMusic;

    [SerializeField] List<string> splashTexts = new List<string>();
    [SerializeField] TMP_Text splash;

    public MainMenu mainMenu;
    private void Awake()
    {
        mainMenu = FindObjectOfType<MainMenu>();        
    }
    private async void Start()
    {
        _networkManager.ClientManager.OnClientConnectionState += UpdateClientConnectionState;
        _networkManager.ServerManager.OnServerConnectionState += UpdateServerConnectionState;


        utp = (FishyUnityTransport)_networkManager.TransportManager.Transport;
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        foreach (Image image in GetComponentsInChildren<Image>())
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }
        foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>())
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }
        menuVideoPlayer.targetCameraAlpha = 0;
        StartCoroutine(WaitToShow());
    }

    void UpdateClientConnectionState(ClientConnectionStateArgs args)
    {
        switch (args.ConnectionState.ToString())
        {
            case "Stopped":
                clientButtonText.text = "Start\nClient";
                break;
            case "Starting":
                clientButtonText.text = "Starting\nClient";
                break;
            case "Stopping":
                clientButtonText.text = "Stopping\nClient";
                break;
            case "Started":
                clientButtonText.text = "Stop\nClient";
                break;

            default:
                clientButtonText.text = "Wake up!";

                break;

        }



    }
    void UpdateServerConnectionState(ServerConnectionStateArgs args)
    {
        switch (args.ConnectionState.ToString())
        {
            case "Stopped":
                serverButtonText.text = "Start\nServer";
                break;
            case "Starting":
                serverButtonText.text = "Starting\nServer";
                break;
            case "Stopping":
                serverButtonText.text = "Stopping\nServer";
                break;
            case "Started":
                serverButtonText.text = "Stop\nServer";
                break;

            default:
                serverButtonText.text = "Wake up!";
                break;

        }
    }

    private void OnDestroy()
    {
        _networkManager.ClientManager.OnClientConnectionState -= UpdateClientConnectionState;
        _networkManager.ServerManager.OnServerConnectionState -= UpdateServerConnectionState;

    }

    [SerializeField]
    float waitToStartTime = 6;
    [SerializeField] float subtitleWait = 170000f;
    public float timePassed;
    IEnumerator WaitToShow()
    {

        yield return new WaitForSeconds(waitToStartTime);
        splash.text = SplashText.GetInstance().Get();
        ambientMusic.Play();
        float elapsedTime = 0f;


        while (ambientMusic.timeSamples < subtitleWait)
        {
            foreach (Image image in GetComponentsInChildren<Image>(true))
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(0, 1, (ambientMusic.timeSamples / subtitleWait)));
            }
            foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>(true))
            {
                if (text.gameObject.name == "Placeholder")
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(0, 0.35f, (ambientMusic.timeSamples / subtitleWait)));

                }
                else if (text.gameObject.name == "Subtitle")
                {
                    if (ambientMusic.timeSamples >= subtitleWait)
                        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

                }
                else
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(0, 1, (ambientMusic.timeSamples / subtitleWait)));

                }
            }
            foreach (Button button in GetComponentsInChildren<Button>(true))
            {
                button.interactable = true;
            }
            foreach (TMP_InputField field in GetComponentsInChildren<TMP_InputField>(true))
            {
                if(!serverStarted)
                    field.interactable = true;
            }
            if (menuVideoPlayer != null && menuVideoPlayer.clip != null)
            {
                menuVideoPlayer.targetCameraAlpha = (ambientMusic.timeSamples / subtitleWait);
            }

            elapsedTime += Time.fixedDeltaTime;
            timePassed = ambientMusic.timeSamples;
            yield return null;
        }

        foreach (Image image in GetComponentsInChildren<Image>(true))
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        }
        foreach (TMP_Text text in GetComponentsInChildren<TMP_Text>(true))
        {
            if (text.gameObject.name == "Placeholder")
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0.35f);

            }
            else
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

            }
        }
        if (menuVideoPlayer != null)
        {
            menuVideoPlayer.targetCameraAlpha = 1;
        }
    }

    public async void CreateRelay()
    {
        //clientButtonText.text = "Starting\nClient";

        if (!serverStarted)
        {
            serverButtonText.text = "Starting\nServer";
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(20);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                utp.SetRelayServerData(new RelayServerData(allocation, "dtls"));

                Debug.Log(joinCode);
                joinCodeBox.text = joinCode;
                joinCodeBox.interactable = false;
                this.joinCode = joinCode;
                // Start Server Connection
                _networkManager.ServerManager.StartConnection();
                serverStarted = true;

                // Start Client Connection
                JoinRelay();

                //serverButtonText.text = "Stop\nServer";
                //clientButtonText.text = "Stop\nClient";


            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                StartCoroutine(ShowServerFailedToStartErrorText());

                serverStarted = false;
            }



        }
        else
        {

            joinCodeBox.interactable = true;
            serverStarted = false;

            //  serverButtonText.text = "Start\nServer";

            //JoinRelay();
            clientStarted = false;
            _networkManager.ClientManager.StopConnection();
            //   clientButtonText.text = "Start\nClient";

            _networkManager.ServerManager.StopConnection(true);





        }


    }



    public async void JoinRelay()
    {

        if (!clientStarted)
        {
            clientButtonText.text = "Starting\nClient";

            if (joinCode.Length != 6)
            {
                Debug.LogWarning("No joinCode provided, or code is invalid! Code:" + joinCode);
                StartCoroutine(ShowJoinCodeErrorText());
                return;
            }

            try
            {
                Debug.Log("Joining Relay with " + joinCode);
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                print(joinAllocation.ToString());

                if (!serverStarted)
                {
                    utp.SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
                }
                joinCodeBox.interactable = false;

                if (_networkManager.ClientManager.StartConnection())
                {


                    //        clientButtonText.text = "Stop\nClient";

                    clientStarted = true;



                    if (!inputManager.menuUp)
                    {
                        Cursor.visible = false;
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                StartCoroutine(ShowJoinCodeErrorText());
                clientStarted = false;
            }



        }
        else
        {
            clientStarted = false;
            joinCodeBox.interactable = true;
            _networkManager.ClientManager.StopConnection();
            //  clientButtonText.text = "Start\nClient";


        }

        if (clientStarted)
            SetNetUIVisability(optionsMenu.activeInHierarchy);

    }



    public void SyncJoinCode(string s)
    {
        joinCode = s;

    }

    [SerializeField]
    GameObject[] hidables;

    public void SetNetUIVisability(bool setActive)
    {
        foreach (GameObject hidable in hidables)
        {
            if (hidable.name == "Respawn Button")
            {
                if(!mainMenu.cockpitDestroyed && !mainMenu.mainBodyDestroyed)
                {
                    continue;
                }
            }
            
            
            hidable.SetActive(setActive);
            Cursor.visible = setActive;
            Cursor.lockState = setActive ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    IEnumerator ShowJoinCodeErrorText()
    {
        clientButtonText.text = "Invalid\nCode!";
        yield return new WaitForSeconds(2f);
        clientButtonText.text = "Start\nClient";
    }
    IEnumerator ShowServerFailedToStartErrorText()
    {
        clientButtonText.text = "Server Failed\nto Start!";
        yield return new WaitForSeconds(2f);
        clientButtonText.text = "Start\nServer";
    }

    public void StartRespawn()
    {
        StartCoroutine(Respawn());
    }
    public IEnumerator Respawn()
    {
        JoinRelay();
        yield return new WaitUntil(() => clientStarted == false);
        JoinRelay();
    }

} 