using System.Collections.Generic;
using System.Linq;
using DarkRift;
using UnityEngine;

struct ReconciliationInfo
{
    public uint Frame;
    public NetworkingData.PlayerStateData Data;
    public NetworkingData.PlayerInputData Input;
    
    public ReconciliationInfo(uint frame, NetworkingData.PlayerStateData data, NetworkingData.PlayerInputData input)
    {
        Frame = frame;
        Data = data;
        Input = input;
    }
}

[RequireComponent(typeof(PlayerLogic))]
[RequireComponent(typeof(PlayerInterpolation))]
public class ClientPlayer : MonoBehaviour
{
    private PlayerLogic playerLogic;
    private PlayerInterpolation interpolation;
    private Queue<ReconciliationInfo> reconciliationHistory = new Queue<ReconciliationInfo>();
    
    private ushort id;
    private string playerName;
    private bool isOwn;

    private Vector2 moveDirection;

    private bool stopSent = false;

    void Awake()
    {
        playerLogic = GetComponent<PlayerLogic>();
        interpolation = GetComponent<PlayerInterpolation>();
    }
    
    public void Initialize(ushort id, string playerName, Vector2 position)
    {
        this.id = id;
        this.playerName = playerName;
        //NameText.text = this.playerName;
        if (ConnectionManager.Instance.PlayerId == id)
        {
            Debug.Log($"Initializing our player {playerName} with client id ({id})");
            isOwn = true;
            
            interpolation.CurrentData = new NetworkingData.PlayerStateData(id, position, 0 );
        }
    }
    
    public void OnServerDataUpdate(NetworkingData.PlayerStateData data)
    {
        if (isOwn)
        {
            //Debug.Log($"server says your position is: {data.Position.x}, {data.Position.y}");
            while (reconciliationHistory.Any() && reconciliationHistory.Peek().Frame < GameManager.Instance.LastReceivedServerTick)
            {
                reconciliationHistory.Dequeue();
            }

            if (reconciliationHistory.Any() && reconciliationHistory.Peek().Frame == GameManager.Instance.LastReceivedServerTick)
            {
                ReconciliationInfo info = reconciliationHistory.Dequeue();
                if (Vector2.Distance(info.Data.Position, data.Position) > 0.05f)
                {

                    List<ReconciliationInfo> infos = reconciliationHistory.ToList();
                    interpolation.CurrentData = data;
                    transform.position = data.Position;
                    for (int i = 0; i < infos.Count; i++)
                    {
                        NetworkingData.PlayerStateData u = playerLogic.GetNextFrameData(interpolation.CurrentData.Id, infos[i].Input);
                        interpolation.SetFramePosition(u);
                    }
                }
            }
        }
        else
        {
            interpolation.SetFramePosition(data);
        }
    }

    void FixedUpdate()
    {
        if (isOwn)
        {
            bool[] inputs = new bool[6];
            inputs[0] = Input.GetKey(KeyCode.W);
            inputs[1] = Input.GetKey(KeyCode.A);
            inputs[2] = Input.GetKey(KeyCode.S);
            inputs[3] = Input.GetKey(KeyCode.D);
            inputs[4] = Input.GetKey(KeyCode.Space);
            
            NetworkingData.PlayerInputData inputData = new NetworkingData.PlayerInputData(inputs, 0, 0);

            /*if (inputs.Contains(true))
            {
                stopSent = false;
            */
                transform.position = interpolation.CurrentData.Position;
                NetworkingData.PlayerStateData nextStateData = playerLogic.GetNextFrameData(interpolation.CurrentData.Id, inputData);
                interpolation.SetFramePosition(nextStateData);

                //Debug.Log($"[{interpolation.CurrentData.Position.X}, {interpolation.CurrentData.Position.Y}] => [{nextStateData.Position.X}, {nextStateData.Position.Y}]");

                using (Message message = Message.Create((ushort) NetworkingData.Tags.GamePlayerInput, inputData))
                {
                    ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
                }
                
                reconciliationHistory.Enqueue(new ReconciliationInfo(GameManager.Instance.ClientTick, nextStateData, inputData));

            /*}
            else
            {
                if (!stopSent)
                {
                    using (Message message = Message.Create((ushort) NetworkingData.Tags.GamePlayerInput, inputData))
                    {
                        Debug.Log("sending stop");
                        ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
                    }
                    stopSent = true;
                }
            }*/
        }
    }
}