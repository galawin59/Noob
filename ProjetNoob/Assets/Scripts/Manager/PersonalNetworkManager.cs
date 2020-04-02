using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PersonalNetworkManager : NetworkManager
{
    public static bool isSERVER = false;
    public override void OnServerConnect(NetworkConnection conn)
    {
        GetComponent<NetworkManagerHUD>().showGUI = false;
        Debug.Log("A client connected to the server: " + conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, newPlayer, playerControllerId);
        
        Debug.Log("Player has join the server" + conn);
    }

    public void CreateServer()
    {
        if (!NetworkClient.active && !NetworkServer.active && matchMaker == null)
        {
            isSERVER = true;
            NetworkManager.singleton.StartHost();

        }
    }

    public void JoinServer()
    {
        if (!NetworkClient.active && !NetworkServer.active && matchMaker == null)
        {
            isSERVER = false;
            NetworkManager.singleton.StartClient();
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        Debug.Log("Server is ready");
    }

    public override void OnStartHost()
    {
        Debug.Log("Host has started");
    }

    public override void OnStartServer()
    {
        Debug.Log("Server has started");
    }

    public override void OnStopServer()
    {
        Debug.Log("Server has stopped");
    }

    public override void OnStopHost()
    {
        Debug.Log("Host has stopped");
    }


    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("Connected successfully to server, now to set up other stuff for the client...");
        ClientScene.AddPlayer(0);
    }

    public override void OnStartClient(NetworkClient client)
    {
        Debug.Log("Client has started");
    }

    public void HostGame()
    {
        if (!NetworkClient.active && !NetworkServer.active && matchMaker == null)
        {         
            StartHost();
            Debug.Log(networkAddress);
        }

    }

}
