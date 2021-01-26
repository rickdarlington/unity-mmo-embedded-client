using System.Collections.Generic;
using DarkRift;
using DarkRift.Client;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Dictionary<ushort, ClientPlayer> players = new Dictionary<ushort, ClientPlayer>();
    private Buffer<NetworkingData.GameUpdateData> gameUpdateDataBuffer = new Buffer<NetworkingData.GameUpdateData>(1, 1);

    [Header("Prefabs")] public GameObject PlayerPrefab;

    public uint ClientTick { get; private set; }
    public uint LastReceivedServerTick { get; private set; }

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
        ConnectionManager.Instance.Client.MessageReceived += onMessage;
            
        using (Message message = Message.Create((ushort)NetworkingData.Tags.PlayerReady, new NetworkingData.PlayerReadyData(true)))
        {
            ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
            Debug.Log("Ready message sent.");
        }
    }

    void onMessage(object sender, MessageReceivedEventArgs args)
    {
        using (Message message = args.GetMessage())
        {
            switch ((NetworkingData.Tags) message.Tag)
            {
                case NetworkingData.Tags.GameStartData:
                    Debug.Log("Got game start data.");
                    OnGameStart(message.Deserialize<NetworkingData.GameStartData>());
                    break;
                case NetworkingData.Tags.GameUpdate:
                    Debug.Log("Got game update data.");
                    OnGameUpdate(message.Deserialize<NetworkingData.GameUpdateData>());
                    break;
                default: 
                    Debug.Log($"Unhandled tag: {message.Tag}");
                    break;
            }
        }
    }

    void OnGameUpdate(NetworkingData.GameUpdateData data)
    {
        gameUpdateDataBuffer.Add(data);
    }

    void OnGameStart(NetworkingData.GameStartData data)
    {
        LastReceivedServerTick = data.OnJoinServerTick;
        ClientTick = data.OnJoinServerTick;
        foreach (NetworkingData.PlayerSpawnData playerSpawnData in data.Players)
        {
            SpawnPlayer(playerSpawnData);
        }
    }

    void SpawnPlayer(NetworkingData.PlayerSpawnData data)
    {
        GameObject go = Instantiate(PlayerPrefab);
        ClientPlayer player = go.GetComponent<ClientPlayer>();
        player.Initialize(data.Id, data.Name);
        players.Add(data.Id, player);
        
    }

    void OnDestroy()
    {
        Instance = null;
    }
    
    
    void FixedUpdate()
    {
        ClientTick++;
        NetworkingData.GameUpdateData[] receivedGameUpdateData = gameUpdateDataBuffer.Get();
        foreach (NetworkingData.GameUpdateData data in receivedGameUpdateData)
        {
            //UpdateClientGameState(data);
        }
    }

    void UpdateClientGameState(NetworkingData.GameUpdateData gameUpdateData)
    {
        LastReceivedServerTick = gameUpdateData.Frame;
        foreach (NetworkingData.PlayerSpawnData data in gameUpdateData.SpawnData)
        {
            if (data.Id != ConnectionManager.Instance.PlayerId)
            {
                SpawnPlayer(data);
            }
        }

        foreach (NetworkingData.PlayerDespawnData data in gameUpdateData.DespawnData)
        {
            if (players.ContainsKey(data.Id))
            {
                Destroy(players[data.Id].gameObject);
                players.Remove(data.Id);
            }
        }

        foreach (NetworkingData.PlayerStateData data in gameUpdateData.UpdateData)
        {
            ClientPlayer p;
            if (players.TryGetValue(data.Id, out p))
            {
                p.OnServerDataUpdate(data);
            }
        }
    }
}