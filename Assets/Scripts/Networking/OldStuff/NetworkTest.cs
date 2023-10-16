using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkTest : MonoBehaviour
{
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    

    }

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
      

    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
