using System;
using DarkRift;
using DarkRift.Client;
using DarkRift.Client.Unity;
using UnityEngine;

[RequireComponent(typeof(UnityClient))]
public class ConnectionManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private LoginManager loginManager;
    
    public ushort PlayerId { get; set; }
    public static ConnectionManager Instance;

    public UnityClient Client { get; private set; }
    public delegate void OnConnectedDelegate();
    public event OnConnectedDelegate OnConnected;
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }
    
    void Start()
    {
        Client = GetComponent<UnityClient>();
        Client.ConnectInBackground(Client.Address, Client.Port, true, ConnectCallback);
        Client.Disconnected += DisconnectCallback;
    }


    private void ConnectCallback(Exception exception)
    {
        if (Client.ConnectionState == ConnectionState.Connected)
        {
            OnConnected?.Invoke();
        }
        else
        {
            Debug.LogError("Unable to connect to server.");
        }
    }

    private void DisconnectCallback(object o, DisconnectedEventArgs args)
    {
        Debug.LogError("Server disconnected.");
        loginManager.StartLoginProcess();
    }
}
