using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour
{
    public Button hostButton;
    public Button serverButton;
    public Button clientButton;

    //LISTA DE CLIENTES PARA CONECTAR
    private List<ulong> connectedClients = new List<ulong>();

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
        //COMPROBACIÓN DE SI HAY 3 O MÁS CLIENTES, ENTONCES CARGA EL JUEGO.
        if (connectedClients.Count >= 3)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
        }
    }

    private void SetInteractable(bool v)
    {
        hostButton.interactable = v;
        serverButton.interactable = v;
        clientButton.interactable = v;
    }

    private void OnServerStarted()
    {
        print("Server Tomi");
        if (NetworkManager.Singleton.IsServer)
        {
            LoadScene();
        }
    }

    private void OnClientConnectedCallback(ulong id)
    {
        print("Client connected " + id);

        //AGREGA UN CLIENTE DE LA LISTA DE CLIENTS
        connectedClients.Add(id);
        LoadScene();
    }

    private void OnTransportFailure()
    {
        print("Error de conexión!! Debe haber al menos 3 clientes conectados antes de iniciar la partida.");
        SetInteractable(true);
    }
}