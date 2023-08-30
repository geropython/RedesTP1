using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviour
{
    public Button hostButton;
    public Button serverButton;
    public Button clientButton;

    public void OnHost()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
        SetInteractable(false);
        NetworkManager.Singleton.StartHost();
    }

    public void OnServer()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
        SetInteractable(false);
        NetworkManager.Singleton.StartServer();
    }

    public void OnClient()
    {
        NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
        SetInteractable(false);
        NetworkManager.Singleton.StartClient();
    }

    private void LoadScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
    }

    private void SetInteractable(bool v)
    {
        hostButton.interactable = v;
        serverButton.interactable = v;
        clientButton.interactable = v;
    }

    private void OnServerStarted()
    {
        print("Server Seba");
        if (NetworkManager.Singleton.IsServer)
        {
            LoadScene();
        }
    }

    private void OnClientConnectedCallback(ulong id)
    {
        print("Client connected " + id);
    }

    private void OnTransportFailure()
    {
        print("Failure!!!");
        SetInteractable(true);
    }
}